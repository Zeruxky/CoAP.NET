// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;
    using System.Buffers.Binary;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Common.Extensions;
    using WorldDirect.CoAP.Messages;
    using WorldDirect.CoAP.Messages.Codes;
    using WorldDirect.CoAP.Messages.Options;

    public class CoapOptionRegistry
    {
        private readonly Dictionary<int, IOptionFactory> factories;

        public CoapOptionRegistry(IEnumerable<IOptionFactory> factories)
        {
            this.factories = factories.ToDictionary(i => i.Number);
        }

        public IOptionFactory GetFactory(int number)
        {
            if (this.factories.ContainsKey(number))
            {
                return this.factories[number];
            }

            return new UnknownFactory();
        }
    }

    public class UnknownFactory : IOptionFactory
    {
        public ICoapOption CreateOption(OptionsReader reader)
        {
            var number = reader.PeakNumber();
            var length = reader.ReadLength();
            var value = reader.Read(length);
            return new UnknownOption(number, value);
        }

        public int Number => -1;
    }

    /// <summary>
    /// Provides functionality to deserialize <see cref="CoapMessage"/>s that are specified
    /// by the RFC 7252.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.CoapMessageDeserializer" />
    public sealed class CoapMessageV1Deserializer : CoapMessageDeserializer
    {
        private const byte MASK_VERSION = 0xC0;
        private const byte MASK_TYPE = 0x30;
        private const byte MASK_TOKEN_LENGTH = 0x0F;
        private const byte MASK_CODE_CLASS = 0xE0;
        private const byte MASK_CODE_DETAIL = 0x1F;
        private const byte PAYLOAD_MARKER = 0xFF;
        private const byte MASK_LENGTH = 0x0F;
        private const byte MASK_DELTA = 0xF0;

        private bool disposed = false;
        private bool hasPayload;
        private readonly TransformBlock<Memory<byte>, IDeserializerResult<CoapMessage>> deserializerBlock;
        private readonly IDisposable inputLink;
        private readonly IDisposable outputLink;
        private IEnumerable<IOptionFactory> factories;
        private CoapOptionRegistry optionRegistry;

        public CoapMessageV1Deserializer(CodeRegistry registry, CoapOptionRegistry optionRegistry)
            : base(registry, factory)
        {
            this.optionRegistry = optionRegistry;
            this.deserializerBlock = new TransformBlock<Memory<byte>, IDeserializerResult<CoapMessage>>(this.Deserialize);
            var linkOptions = new DataflowLinkOptions()
            {
                PropagateCompletion = true,
            };
            this.inputLink = this.InputBlock.LinkTo(this.deserializerBlock, b => (CoapVersion)(UInt2)((b.Span[0] & MASK_VERSION) >> 6) == CoapVersion.V1);
            this.outputLink = this.deserializerBlock.LinkTo(this.OutputBlock, linkOptions);
        }

        public void Dispose()
        {
            this.inputLink?.Dispose();
            this.outputLink?.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.inputLink?.Dispose();
                this.outputLink?.Dispose();
            }

            this.disposed = true;
            base.Dispose(disposing);
        }

        internal CoapHeader GetHeader(Span<byte> value)
        {
            var version = new CoapVersion((UInt2)((value[0] & MASK_VERSION) >> 6));
            if (!version.Equals(CoapVersion.V1))
            {
                throw new InvalidOperationException($"Can not serialize CoAP message with version {version}.");
            }

            var type = new CoapType((UInt2)((value[0] & MASK_TYPE) >> 4));
            var tokenLength = new CoapTokenLength((UInt4)(value[0] & MASK_TOKEN_LENGTH));
            if ((UInt4)tokenLength > 8)
            {
                throw new MessageFormatErrorException("Token Lengths between 9 and 15 are reserved.");
            }

            var @class = new CodeClass((UInt3)((value[1] & MASK_CODE_CLASS) >> 5));
            var detail = new CodeDetail((UInt5)(value[1] & MASK_CODE_DETAIL));
            var code = this.Registry.Codes.SingleOrDefault(c => c.Class.Equals(@class) && c.Detail.Equals(detail));
            if (code == null)
            {
                throw new MessageFormatErrorException("Unknown or reserved code found.");
            }

            var messageId = new CoapMessageId(BinaryPrimitives.ReadUInt16BigEndian(value.Slice(2, 2)));
            this.position = 4;
            return new CoapHeader(version, type, tokenLength, code, messageId);
        }

        internal CoapToken GetToken(Span<byte> value, CoapTokenLength tokenLength)
        {
            ulong tokenValue = 0;
            var length = (UInt4)tokenLength;
            if (length == 1)
            {
                tokenValue = value[this.position];
            }

            if (length >= 2 && length < 4)
            {
                tokenValue = BinaryPrimitives.ReadUInt16LittleEndian(value.Slice(0, length));
            }

            if (length >= 4 && length < 8)
            {
                tokenValue = BinaryPrimitives.ReadUInt32LittleEndian(value.Slice(0, length));
            }

            if (length == 8)
            {
                tokenValue = BinaryPrimitives.ReadUInt64LittleEndian(value.Slice(0, length));
            }

            this.position += length;
            return new CoapToken(tokenValue, tokenLength);
        }

        internal OptionCollection GetOptions(Span<byte> value)
        {
            var options = new List<ICoapOption>();
            using (var stream = new MemoryStream(value.ToArray()))
            using (var reader = new OptionsReader(stream))
            {
                while (reader.HasNext())
                {
                    var number = reader.PeakNumber();
                    var factory = this.optionRegistry.GetFactory(number);
                    options.Add(factory.CreateOption(reader));
                }
            }

            return new OptionCollection(options);
        }

        internal ICoapOption GetOption(Span<byte> value, ICoapOption previousOption)
        {
            var delta = (ushort)((value[0] & MASK_DELTA) >> 4);
            var length = (ushort)(value[0] & MASK_LENGTH);
            var lengthOfExtendedDelta = 0;
            if (delta == 15 && length != 15)
            {
                throw new MessageFormatErrorException("Delta is not allowed to be 15 (0xFF), because it is reserved for payload marker.");
            }

            if (delta == 14)
            {
                delta = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(value.Slice(1, 2)) - 269);
                lengthOfExtendedDelta = 2;
            }

            if (delta == 13)
            {
                delta = (ushort)(value[1] - 13);
                lengthOfExtendedDelta = 1;
            }

            if (length == 15)
            {
                throw new MessageFormatErrorException("Length 15 (0xFF) is reserved for future use.");
            }

            var lengthOfExtendedLength = 0;
            if (length == 14)
            {
                length = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(value.Slice(lengthOfExtendedDelta + 1, 2)) - 269);
                lengthOfExtendedLength = 2;
            }

            if (length == 13)
            {
                length = (ushort)(value[lengthOfExtendedDelta + 1] - 13);
                lengthOfExtendedLength = 1;
            }

            var optionValue = value.Slice(1 + lengthOfExtendedDelta + lengthOfExtendedLength, length);
            var number = previousOption == null
                ? delta
                : (ushort)(previousOption.Number + delta);
            var factory = this.factories.SingleOrDefault(f => f.CanCreate(value.ToArray(), delta));
            var option = factory?.CreateOption(value.ToArray());
            return option;
        }

        private IDeserializerResult<CoapMessage> Deserialize(Memory<byte> value)
        {
            this.hasPayload = false;
            var position = 0;

            try
            {
                var x = new BinaryReader();
                var header = this.GetHeader(value.Span.Slice(0, 4));
                position += 4;

                var token = this.GetToken(value.Span.Slice(position, (UInt4)header.Length), header.Length);
                position += (UInt4)token.Length;

                var options = this.GetOptions(value.Span.Slice(position));
                position += options.RawSize;

                var payload = position > value.Length
                    ? new CoapPayload(value.Span.Slice(position))
                    : CoapPayload.EmptyPayload;

                var message = new CoapMessage(header, token, options, payload);
                return MessageDeserializerResult.FromMessage(message);
            }
            catch (Exception e)
            {
                return MessageDeserializerResult.FromException(e);
            }
        }

        private bool CanReadNextOption(Span<byte> value)
        {
            if (value.Length == 0)
            {
                return false;
            }

            if (value[0] == PAYLOAD_MARKER)
            {
                if (value.Length == 1)
                {
                    throw new MessageFormatErrorException("Found payload marker, but apparently no payload is given.");
                }

                this.hasPayload = true;
                return false;
            }

            return true;
        }
    }

    public class UriHostFactory : IOptionFactory
    {
        public ICoapOption CreateOption(OptionsReader reader)
        {
            var value = reader.ReadString();
            return new UriHost(value);
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

    public class OptionsReader : IDisposable
    {
        private readonly Stream stream;
        private int offset = 0;

        public OptionsReader(Stream stream)
        {
            this.stream = stream;
        }

        public int PeakNumber()
        {
            var delta = this.stream.ReadByte();
            this.stream.Seek(-1, SeekOrigin.Current);
            return this.offset + delta;
        }

        public void Dispose()
        {
            this.stream?.Dispose();
        }
    }

    public interface IOptionFactory
    {
        ICoapOption CreateOption(OptionsReader reader);

        int Number { get; }
    }

    public abstract class DeserializerResult<TValue> : IDeserializerResult<TValue>
    {
        protected DeserializerResult(TValue value, Exception exception)
        {
            this.Value = value;
            this.CausingException = exception;
        }

        public Exception CausingException { get; }

        public TValue Value { get; }

        public bool Failed => this.CausingException != null;

        public int Position { get; set; }
    }

    public class OptionsParser : IEnumerable<ICoapOption>
    {
        public IEnumerator<ICoapOption> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class OptionSerializer
    {
        public 
    }

    public class OptionCollection
    {
        private List<ICoapOption> options;

        public OptionCollection(IEnumerable<ICoapOption> options)
        {
            this.options = new List<ICoapOption>(options);
            this.IsReadOnly = true;
        }

        public int RawSize { get; set; }

        public bool IsReadOnly { get; set; }

        public IList<ICoapOption> Options => this.options;

        public void Add(ICoapOption option)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException();
            }

            this.options.Add(option);
        }

        public void AddRange(IEnumerable<ICoapOption> option)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException();
            }

            this.options.AddRange(option);
        }

        public IOrderedEnumerable<T> Get<T>()
        {

        }
    }

    public class CoapRequest
    {
        public OptionCollection Options { get; set; }

        public Uri Path => new Uri(System.IO.Path.Combine(Options.Get<UriPath>().Select(u => u.Value)));
    }
}
