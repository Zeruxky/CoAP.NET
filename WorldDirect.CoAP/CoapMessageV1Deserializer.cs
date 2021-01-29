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
        private readonly V1OptionsReader optionReader;
        private readonly PayloadReader payloadReader;
        private readonly V1HeaderReader headerReader;
        private readonly V1TokenReader tokenReader;

        public CoapMessageV1Deserializer(V1HeaderReader headerReader, V1TokenReader tokenReader, V1OptionsReader optionsReader, PayloadReader payloadReader)
        {
            this.headerReader = headerReader;
            this.tokenReader = tokenReader;
            this.optionReader = optionsReader;
            this.payloadReader = payloadReader;
        }

        public CoapMessage Deserialize(ReadOnlyMemory<byte> value)
        {
            var position = this.headerReader.Read(value.Slice(0, 4), out var header);
            position += this.tokenReader.Read(value.Slice(position, (UInt4)header.Length), out var token);
            position += this.optionReader.Read(value.Slice(position), out var options);
            position += this.payloadReader.Read(value.Slice(position), out var payload);
            return new CoapMessage(header, token, options, payload);
        }

        public bool CanDeserialize(CoapVersion version)
        {
            return version.Equals(CoapVersion.V1);
        }
    }

    public interface IReader<TResult>
    {
        int Read(ReadOnlyMemory<byte> value, out TResult result);
    }
    
    public interface IPayloadReader
    {
        byte[] Read(ReadOnlySpan<byte> value, out int readBytes);
    }

    public class PayloadReader : IReader<ReadOnlyMemory<byte>>
    {
        /// <inheritdoc />
        public int Read(ReadOnlyMemory<byte> value, out ReadOnlyMemory<byte> result)
        {
            if (value.IsEmpty)
            {
                result = new byte[0];
                return 0;
            }

            if (value.Span[0] != 0xFF || value.Length == 1)
            {
                throw new MessageFormatErrorException("Payload marker found but no payload.");
            }

            result = value.Slice(1);
            return value.Length;
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

    public class V1TokenReader : IReader<CoapToken>
    {
        public int Read(ReadOnlyMemory<byte> value, out CoapToken result)
        {
            var length = value.Length;
            var buffer = new byte[8];
            var src = BitConverter.IsLittleEndian ? value.ToArray().Reverse().ToArray() : value.ToArray();
            Array.Copy(src, 0, buffer, buffer.Length - src.Length, src.Length);

            var tokenValue = BinaryPrimitives.ReadUInt64BigEndian(new ReadOnlySpan<byte>(buffer));
            result = new CoapToken(tokenValue, (CoapTokenLength)(UInt4)length);
            return length;
        }
    }

    public class V1HeaderReader : IReader<CoapHeader>
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

        public int Read(ReadOnlyMemory<byte> value, out CoapHeader result)
        {
            var version = (CoapVersion)(UInt2)((value.Span[0] & MASK_VERSION) >> 6);
            if (!this.CanRead(version))
            {
                throw new ArgumentException("Can only read headers for Version 1.", nameof(version));
            }

            var type = (CoapType)(UInt2)((value.Span[0] & MASK_TYPE) >> 4);
            var tokenLength = (CoapTokenLength)(UInt4)(value.Span[0] & MASK_TOKEN_LENGTH);
            if ((UInt4)tokenLength > 8)
            {
                throw new MessageFormatErrorException("Token lengths of 9 to 15 are reserved.");
            }

            var codeClass = (CodeClass)(UInt3)((value.Span[1] & MASK_CODE_CLASS) >> 5);
            var codeDetail = (CodeDetail)(UInt5)(value.Span[1] & MASK_CODE_DETAIL);
            var code = this.registry.Get(codeClass, codeDetail);
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code), $"Unknown code {codeClass}.{codeDetail}.");
            }

            var messageId = (CoapMessageId)BinaryPrimitives.ReadUInt16BigEndian(value.Span.Slice(2, 2));

            result = new CoapHeader(version, type, tokenLength, code, messageId);
            return 4;
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

    public class V1OptionsReader : IReader<IReadOnlyCollection<ICoapOption>>
    {
        private const byte MASK_DELTA = 0xF0;
        private const byte MASK_LENGTH = 0x0F;
        private const byte PAYLOAD_MARKER = 0xFF;

        private readonly Dictionary<int, IOptionFactory> factories;

        public V1OptionsReader(IEnumerable<IOptionFactory> factories)
        {
            this.factories = factories.ToDictionary(f => f.Number);
        }

        public int Read(ReadOnlyMemory<byte> value, out IReadOnlyCollection<ICoapOption> result)
        {
            var options = new List<ICoapOption>();
            var offset = 0;
            var previousNumber = 0;
            while (this.HasNext(value.Span.Slice(offset)))
            {
                var optionData = this.GetOptionData(previousNumber, value.Span.Slice(offset), out var bytesConsumed);
                previousNumber += optionData.Number;
                offset += bytesConsumed;

                var factory = this.factories.ContainsKey(optionData.Number)
                    ? this.factories[optionData.Number]
                    : new UnknownFactory();
                var option = factory.Create(optionData);

                options.Add(option);
            }

            result = new ReadOnlyCollection<ICoapOption>(options);
            return offset;
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
    }

    public interface IOptionFactory
    {
        ICoapOption Create(OptionData src);

        int Number { get; }
    }
}
