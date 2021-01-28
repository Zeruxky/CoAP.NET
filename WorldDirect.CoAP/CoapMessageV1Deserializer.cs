// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;
    using System.Buffers.Binary;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using WorldDirect.CoAP.Messages;
    using WorldDirect.CoAP.Messages.Codes;
    using WorldDirect.CoAP.Messages.Options;

    public class UnknownFactory : IOptionFactory
    {
        public ICoapOption Create(OptionData src)
        {
            return new UnknownOption(src.Number, src.Value.ToArray());
        }

        public int Number => -1;
    }

    public class IfMatchFactory : IOptionFactory
    {
        public ICoapOption Create(OptionData src)
        {
            return new IfMatch(src.Value.ToArray());
        }

        public int Number => 1;
    }

    /// <summary>
    /// Provides functionality to deserialize <see cref="CoapMessage"/>s that are specified
    /// by the RFC 7252.
    /// </summary>
    public sealed class CoapMessageV1Deserializer : IMessageDeserializer
    {
        private readonly IOptionsReader optionReader;
        private readonly IHeaderReader headerReader;
        private readonly ITokenReader tokenReader;

        public CoapMessageV1Deserializer(IEnumerable<IHeaderReader> headerReaders, IEnumerable<ITokenReader> tokenReaders, IEnumerable<IOptionsReader> optionsReaders)
        {
            this.headerReader = headerReaders.Single(h => h.CanRead(CoapVersion.V1));
            this.tokenReader = tokenReaders.Single(t => t.CanRead(CoapVersion.V1));
            this.optionReader = optionsReaders.Single(o => o.CanRead(CoapVersion.V1));
        }

        public CoapMessage Deserialize(ReadOnlySpan<byte> value)
        {
            var position = 0;
            var header = this.headerReader.Read(value.Slice(position, 4), out var headerBytesRead);
            position += headerBytesRead;
            var token = this.tokenReader.Read(value.Slice(position, (UInt4)header.Length));
            position += (UInt4)token.Length;
            var options = this.optionReader.Read(value.Slice(position), out var optionBytesRead);
            position += optionBytesRead;
            if (position == value.Length)
            {
                return new CoapMessage(header, token, options, new byte[0]);
            }

            // count one byte for payload marker.
            position += 1;
            var payload = value.Slice(position);
            position += payload.Length;
            return new CoapMessage(header, token, options, payload.ToArray());
        }

        public bool CanDeserialize(CoapVersion version)
        {
            return version.Equals(CoapVersion.V1);
        }
    }

    public class UriHostFactory : IOptionFactory
    {
        public ICoapOption Create(OptionData src)
        {
            return new UriHost(src.StringValue);
        }

        public int Number => 7;
    }

    public class UnknownOption : ICoapOption
    {
        public UnknownOption(ushort number, byte[] rawValue)
        {
            this.Number = number;
            this.RawValue = rawValue;
        }

        public ushort Number { get; }

        public byte[] RawValue { get; }
    }

    public interface ITokenReader
    {
        CoapToken Read(ReadOnlySpan<byte> value);

        bool CanRead(CoapVersion version);
    }

    public class V1TokenReader : ITokenReader
    {
        public CoapToken Read(ReadOnlySpan<byte> value)
        {
            var length = (UInt4)value.Length;
            ulong tokenValue = 0;
            if (length == 1)
            {
                tokenValue = value[0];
            }

            if (length == 2)
            {
                tokenValue = BinaryPrimitives.ReadUInt16LittleEndian(value.Slice(0, length));
            }

            if (length > 2 && length <= 4)
            {
                tokenValue = BinaryPrimitives.ReadUInt32LittleEndian(value.Slice(0, length));
            }

            if (length > 4 && length <= 8)
            {
                tokenValue = BinaryPrimitives.ReadUInt64LittleEndian(value.Slice(0, length));
            }

            return new CoapToken(tokenValue, (CoapTokenLength)length);
        }

        public bool CanRead(CoapVersion version)
        {
            return version.Equals(CoapVersion.V1);
        }
    }

    public class V1HeaderReader : IHeaderReader
    {
        private readonly CodeRegistry registry;
        private const byte MASK_VERSION = 0xC0;
        private const byte MASK_TYPE = 0x30;
        private const byte MASK_TOKEN_LENGTH = 0x0F;
        private const byte MASK_CODE_CLASS = 0xE0;
        private const byte MASK_CODE_DETAIL = 0x1F;

        public V1HeaderReader(CodeRegistry registry)
        {
            this.registry = registry;
        }

        public CoapHeader Read(ReadOnlySpan<byte> value, out int readBytes)
        {
            var version = (CoapVersion)(UInt2)((value[0] & MASK_VERSION) >> 6);
            if (!this.CanRead(version))
            {
                throw new ArgumentException("Can only read headers for Version 1.", nameof(version));
            }

            var type = (CoapType)(UInt2)((value[0] & MASK_TYPE) >> 4);
            var tokenLength = (CoapTokenLength)(UInt4)(value[0] & MASK_TOKEN_LENGTH);
            if ((UInt4)tokenLength > 8)
            {
                throw new MessageFormatErrorException("Token lengths of 9 to 15 are reserved.");
            }

            var codeClass = (CodeClass)(UInt3)((value[1] & MASK_CODE_CLASS) >> 5);
            var codeDetail = (CodeDetail)(UInt5)(value[1] & MASK_CODE_DETAIL);
            var code = this.registry.Get(codeClass, codeDetail);
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code), $"Unknown code {codeClass}.{codeDetail}.");
            }

            var messageId = (CoapMessageId)BinaryPrimitives.ReadUInt16BigEndian(value.Slice(2, 2));

            readBytes = 4;
            return new CoapHeader(version, type, tokenLength, code, messageId);
        }

        public bool CanRead(CoapVersion version)
        {
            return version.Equals(CoapVersion.V1);
        }
    }

    public interface IHeaderReader
    {
        CoapHeader Read(ReadOnlySpan<byte> value, out int readBytes);

        bool CanRead(CoapVersion version);
    }

    public interface IOptionsReader
    {
        IReadOnlyCollection<ICoapOption> Read(ReadOnlySpan<byte> value, out int readBytes);

        bool CanRead(CoapVersion version);
    }

    public class V1OptionsReader : IOptionsReader
    {
        private const byte MASK_DELTA = 0xF0;
        private const byte MASK_LENGTH = 0x0F;
        private const byte PAYLOAD_MARKER = 0xFF;

        private readonly Dictionary<int, IOptionFactory> factories;

        public V1OptionsReader(IEnumerable<IOptionFactory> factories)
        {
            this.factories = factories.ToDictionary(f => f.Number);
        }

        public IReadOnlyCollection<ICoapOption> Read(ReadOnlySpan<byte> value, out int readBytes)
        {
            var options = new List<ICoapOption>();
            var offset = 0;
            var previousNumber = 0;
            while (this.HasNext(value.Slice(offset)))
            {
                var optionData = this.GetOptionData(previousNumber, value.Slice(offset), out var bytesConsumed);
                previousNumber += optionData.Number;
                offset += bytesConsumed;

                var factory = this.factories.ContainsKey(optionData.Number)
                    ? this.factories[optionData.Number]
                    : new UnknownFactory();
                var option = factory.Create(optionData);

                options.Add(option);
            }

            // Plus one byte for the payload marker.
            readBytes = offset;
            return new ReadOnlyCollection<ICoapOption>(options);
        }

        public bool CanRead(CoapVersion version)
        {
            return version.Equals(CoapVersion.V1);
        }

        private OptionData GetOptionData(int previousNumber, ReadOnlySpan<byte> src, out int bytesConsumed)
        {
            var delta = this.ReadDelta(src);
            var length = this.ReadLength(src, delta.Size);

            var number = previousNumber + delta.Value;
            var valueOffset = 1 + delta.Size + length.Size;

            bytesConsumed = valueOffset + length.Value;
            return new OptionData((ushort)number, length.Value, src.Slice(valueOffset, length.Value));
        }

        private OptionsLength ReadLength(ReadOnlySpan<byte> value, byte readDeltaBytes)
        {
            var length = (UInt4)(value[0] & MASK_LENGTH);
            var startOfExtendedLength = readDeltaBytes + 1;
            if (length == 15)
            {
                throw new MessageFormatErrorException("Length value of 15 is reserved for future use.");
            }

            if (length == 14)
            {
                var extended = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(value.Slice(startOfExtendedLength, 2)) - 269);
                return new OptionsLength(extended, 2);
            }

            if (length == 13)
            {
                var extended = (ushort)(value[startOfExtendedLength] - 13);
                return new OptionsLength(extended, 1);
            }

            return new OptionsLength(length, 0);
        }

        private OptionsDelta ReadDelta(ReadOnlySpan<byte> value)
        {
            var delta = (UInt4)((value[0] & MASK_DELTA) >> 4);
            var length = (UInt4)(value[0] & MASK_LENGTH);
            if (delta == 15 && length != 15)
            {
                throw new MessageFormatErrorException("Delta value of 15 is reserved for payload marker.");
            }

            if (delta == 14)
            {
                var extended = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(value.Slice(1, 2)) - 269);
                return new OptionsDelta(extended, 2);
            }

            if (delta == 13)
            {
                var extended = (ushort)(value[1] - 13);
                return new OptionsDelta(extended, 1);
            }

            return new OptionsDelta(delta, 0);
        }

        private bool HasNext(ReadOnlySpan<byte> value)
        {
            // reached the end.
            if (value.Length == 0)
            {
                return false;
            }

            // found payload marker.
            if (value[0] == PAYLOAD_MARKER)
            {
                // only the payload marker is left.
                if (value.Length == 1)
                {
                    throw new MessageFormatErrorException("Found payload marker, but no payload.");
                }

                // payload is given.
                return false;
            }

            // found no payload marker but more bytes to read.
            return true;
        }
    }

    public struct OptionsDelta
    {
        public OptionsDelta(ushort value, byte size)
        {
            this.Value = value;
            this.Size = size;
        }

        public ushort Value { get; }

        public byte Size { get; }
    }

    public struct OptionsLength
    {
        public OptionsLength(ushort value, byte size)
        {
            this.Value = value;
            this.Size = size;
        }

        public ushort Value { get; }

        public byte Size { get; }
    }

    public ref struct OptionData
    {
        public OptionData(ushort number, ushort length, ReadOnlySpan<byte> value)
        {
            this.Number = number;
            this.Length = length;
            this.Value = value;
        }

        public ushort Number { get; }

        public ushort Length { get; }

        public ReadOnlySpan<byte> Value { get; }

        public string StringValue
        {
            get
            {
                var chars = MemoryMarshal.Cast<byte, char>(this.Value);
                return chars.ToString();
            }
        }

        public uint UIntValue => BinaryPrimitives.ReadUInt32BigEndian(this.Value);
    }

    public interface IOptionFactory
    {
        ICoapOption Create(OptionData src);

        int Number { get; }
    }
}
