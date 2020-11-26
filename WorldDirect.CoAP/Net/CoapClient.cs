namespace WorldDirect.CoAP.Net
{
    using System;
    using System.Buffers;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using NLog.Targets.Wrappers;

    public static class UriExtensions
    {
        public static bool IsValidCoapUri(this Uri uri)
        {
            if (uri.Scheme.ToLowerInvariant().StartsWith("coap", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (uri.Scheme.ToLowerInvariant().StartsWith("coaps", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }

    public class CoapClient
    {
        private readonly UdpClient listener;
        private Uri uri;

        public CoapClient(string hostname, int port)
        {
            this.listener = new UdpClient(hostname, port);
        }

        public CoapClient(IPEndPoint endpoint)
        {
            this.listener = new UdpClient(endpoint);
        }

        public Uri BaseAddress
        {
            get
            {
                return this.uri;
            }

            set
            {
                if (!value.IsValidCoapUri())
                {
                    throw new ArgumentException();
                }

                this.uri = value;
            }
        }

        public int Port
        {
            get
            {
                return this.BaseAddress.Port == -1
                    ? 5683
                    : this.BaseAddress.Port;
            }
        }

        public async Task<CoapResponseMessage> SendAsync(CoapRequestMessage message, CancellationToken ct)
        {
            var content = message.GetBytes().ToArray();
            var sendBytes = await this.listener.SendAsync(content, content.Length, this.BaseAddress.ToString(), this.Port).ConfigureAwait(false);
            if (sendBytes < 1)
            {
                throw new ArgumentException($"No bytes sent to {this.listener.Client.RemoteEndPoint}.", nameof(sendBytes));
            }

            var result = await this.listener.ReceiveAsync().ConfigureAwait(false);
            var diagram = new UdpDatagram(result);
            var response = new CoapResponseMessage(diagram);

            return response;
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="CoapMessage"/> according to RFC 7252.
    /// </summary>
    public class CoapHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoapHeader"/> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the given <paramref name="bytes"/> are greater than four bytes.</exception>
        public CoapHeader(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), bytes.Length, $"Expecting four bytes, found {bytes.Length} bytes.");
            }

            this.Version = MemoryMarshal.Read<Version>(bytes.Slice(0, 1));
            this.Type = MemoryMarshal.Read<Type>(bytes.Slice(0, 1));
            this.TokenLength = MemoryMarshal.Read<TokenLength>(bytes.Slice(0, 1));
            this.Code = Code.Parse(bytes[1]);
            this.Id = MemoryMarshal.Read<MessageId>(bytes.Slice(2, 2));
        }

        public CoapHeader(Type type, TokenLength tokenLength, Code code, MessageId id)
        {
            this.Version = (Version)0x40;
            this.Type = type;
            this.TokenLength = tokenLength;
            this.Code = code;
            this.Id = id;
        }

        /// <summary>
        /// Gets the <see cref="Version"/> of the <see cref="CoapMessage"/>.
        /// </summary>
        /// <remarks>
        /// The default value for RFC 7252 is always 01 (1). Other values are reserved
        /// for the future.
        /// </remarks>
        public Version Version { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the <see cref="CoapMessage"/>.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the <see cref="TokenLength"/> of the <see cref="CoapMessage"/>.
        /// It indicates the length of the variable-length <see cref="Token"/> field.
        /// </summary>
        /// <remarks>
        /// For RFC 7252 only lengths of 0 - 8 bytes are allowed. Lengths from 9 - 15
        /// are reserved.
        /// </remarks>
        public TokenLength TokenLength { get; }

        /// <summary>
        /// Gets the <see cref="CoAP.Code"/> of the <see cref="CoapMessage"/>.
        /// </summary>
        /// <remarks>
        /// For RFC 7252 <see cref="Code"/>s 1.00 - 1.31 and 6.00 - 7.31 are reserved.
        /// </remarks>
        public Code Code { get; }

        /// <summary>
        /// Gets the <see cref="MessageId"/> of this <see cref="CoapMessage"/>.
        /// </summary>
        public MessageId Id { get; }

        public IEnumerable<byte> GetBytes()
        {
            yield return (byte)(this.Version | this.Type | this.TokenLength);
            yield return this.Code;
            foreach (var b in (byte[])this.Id)
            {
                yield return b;
            }
        }
    }

    /// <summary>
    /// Represents a response message for CoAP.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.Net.CoapMessage" />
    public class CoapResponseMessage : CoapMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoapResponseMessage"/> class.
        /// </summary>
        /// <param name="datagram">The datagram.</param>
        public CoapResponseMessage(UdpDatagram datagram)
            : base(datagram.Result.Buffer)
        {
        }
    }

    /// <summary>
    /// Represents a request message for CoAP.
    /// </summary>
    public class CoapRequestMessage : CoapMessage
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapRequestMessage"/> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public CoapRequestMessage(byte[] bytes)
            : base(bytes)
        {
        }

        public static CoapRequestMessage Create(CoapHeader header, IEnumerable<Token> tokens, IEnumerable<Option> options, byte[] payload)
        {
            var headerBytes = header.GetBytes();
            var tokenBytes = tokens.Select(t => (byte)t);
            var optionBytes = options.SelectMany(o => o.GetBytes());
            var combinedBytes = headerBytes
                .Concat(tokenBytes)
                .Concat(optionBytes)
                .Append<byte>(0xFF)
                .Concat(payload)
                .ToArray();

            var request = new CoapRequestMessage(combinedBytes);
            return request;
        }
    }

    /// <summary>
    /// Represents the base class for a CoAP message according to RFC 7252
    /// page 16, figure 7.
    /// <remarks>
    /// Inherit from this class to implement a concrete type of a CoAP message.
    /// <see cref="CoapRequestMessage"/> represents a request message in CoAP context,
    /// a <see cref="CoapResponseMessage"/> represents a response message in CoAP context.
    /// </remarks>
    /// </summary>
    /// <seealso cref="CoapRequestMessage"/>
    /// <seealso cref="CoapResponseMessage"/>
    public abstract class CoapMessage : IDisposable
    {
        private static readonly List<Version> AllowedVersions = new List<Version>()
        {
            (Version)0x01,
        };

        private readonly IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(65535);
        private readonly IEnumerable<Token> tokens;
        private readonly IEnumerable<Option> options;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapMessage"/> class.
        /// </summary>
        /// <param name="datagram">The datagram that represents the <see cref="CoapMessage"/>.</param>
        protected CoapMessage(byte[] datagram)
        {
            var memory = this.owner.Memory;
            datagram.CopyTo(memory);
            this.Header = new CoapHeader(memory.Span.Slice(0, 4));
            this.tokens = Token.Parse(memory.Span.Slice(4, this.Header.TokenLength));
        }

        /// <summary>
        /// Gets the <see cref="CoapHeader"/> form that <see cref="CoapMessage"/>.
        /// </summary>
        public CoapHeader Header { get; }

        /// <summary>
        /// Gets the <see cref="Token"/>s from that <see cref="CoapMessage"/>.
        /// </summary>
        public IReadOnlyList<Token> Tokens => this.tokens.ToList();

        /// <summary>
        /// Gets the <see cref="Option"/>s from that <see cref="CoapMessage"/>.
        /// </summary>
        public IReadOnlyList<Option> Options => this.Options.OrderBy(o => o.)

        /// <summary>
        /// Gets the appending payload of this <see cref="CoapMessage"/>.
        /// </summary>
        /// <value>
        /// The payload.
        /// </value>
        public byte[] Payload { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CoapMessage"/> should be ignored.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the set <see cref="Version"/> for this <see cref="CoapMessage"/> is 01 (1); otherwise, <c>false</c>.
        /// </value>
        public bool Ignoring => !CoapMessage.AllowedVersions.Contains(this.Header.Version);

        /// <summary>
        /// Gets the underlying bytes that are equivalent to the <see cref="CoapMessage"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with items of type <see cref="byte"/>.</returns>
        public IEnumerable<byte> GetBytes()
        {
            foreach (var b in this.Header.GetBytes())
            {
                yield return b;
            }

            foreach (var token in this.Tokens)
            {
                yield return token;
            }

            foreach (var bytes in this.Options.SelectMany(o => o.GetBytes()))
            {
                yield return bytes;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => this.Dispose(true);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.owner?.Dispose();
            }

            this.disposed = true;
        }
    }

    public class Option : Enumeration
    {
        public static readonly Option Reserved1 = new Option(0, nameof(Reserved1), "Reserved", OptionValueFormat.Empty, 0, 0, 0);
        public static readonly Option IfMatch = new Option(1, nameof(IfMatch), "If-Match", OptionValueFormat.Opaque, 0, 8, -1);
        public static readonly Option UriHost = new Option(3, nameof(UriHost), "Uri-Host", OptionValueFormat.String, 1, 255, );
        public static readonly Option ETag = new Option(4, nameof(ETag), "ETag");
        public static readonly Option IfNoneMatch = new Option(5, nameof(IfNoneMatch), "If-None-Match");
        public static readonly Option UriPort = new Option(7, nameof(UriPort), "Uri-Port");
        public static readonly Option LocationPath = new Option(8, nameof(LocationPath), "Location-Path");
        public static readonly Option UriPath = new Option(11, nameof(UriPath), "Uri-Path");
        public static readonly Option ContentFormat = new Option(12, nameof(ContentFormat), "Content-Format");
        public static readonly Option MaxAge = new Option(14, nameof(MaxAge), "Max-Age");
        public static readonly Option UriQuery = new Option(15, nameof(UriQuery), "Uri-Query");
        public static readonly Option Accept = new Option(17, nameof(Accept), "Accept");
        public static readonly Option LocationQuery = new Option(20, nameof(LocationQuery), "Location-Query");
        public static readonly Option ProxyUri = new Option(35, nameof(ProxyUri), "Proxy-Uri");
        public static readonly Option ProxyScheme = new Option(39, nameof(ProxyScheme), "Proxy-Scheme");
        public static readonly Option Size1 = new Option(60, nameof(Size1), "Size1");
        public static readonly Option Reserved2 = new Option(128, nameof(Reserved2), "Reserved");
        public static readonly Option Reserved3 = new Option(132, nameof(Reserved3), "Reserved");
        public static readonly Option Reserved4 = new Option(136, nameof(Reserved4), "Reserved");
        public static readonly Option Reserved5 = new Option(140, nameof(Reserved5), "Reserved");

        public Option(int id, string name, string displayName, OptionValueFormat format, ushort minLength, ushort maxLength, ushort defaultValue)
            : base(id, name)
        {
            this.DisplayName = displayName;
        }

        public string DisplayName { get; }

        public OptionValueFormat Format { get; }

        public ushort MinLength { get; }

        public ushort MaxLength { get; }

        public ushort DefaultValue { get; }
    }

    //public class Option
    //{
    //    private Option(ReadOnlySpan<byte> bytes)
    //    {
    //        var delta = MemoryMarshal.Read<OptionDelta>(bytes.Slice(0, 1));
    //        if (delta >= 13)
    //        {

    //        }
    //        this.Length = (OptionLength)bytes[0];
    //        this.Value = new byte[255];
    //    }

    //    public OptionDelta Delta { get; }

    //    public OptionLength Length { get; }

    //    public OptionValueFormat Format { get; }

    //    public byte[] Value { get; }

    //    public static explicit operator Option(byte[] bytes) => new Option(bytes);

    //    public IEnumerable<byte> GetBytes()
    //    {
    //        yield return this.Delta;
    //        yield return this.Length;
    //        foreach (var b in this.Value)
    //        {
    //            yield return b;
    //        }
    //    }

    //    private static OptionDelta ReadDelta(ReadOnlySpan<byte> bytes)
    //    {
    //        var delta = MemoryMarshal.Read<OptionDelta>(bytes.Slice(0, 1));
    //        if (delta >= 13)
    //        {

    //            var extendedDelta = MemoryMarshal.Read<OptionDelta>(bytes.Slice(1, 1));
    //        }
    //    }
    //}

    public class OptionValueFormat : Enumeration
    {
        public static readonly OptionValueFormat Empty = new OptionValueFormat(1, nameof(Empty));

        public static readonly OptionValueFormat Opaque = new OptionValueFormat(2, nameof(Opaque));

        public static readonly OptionValueFormat Uint = new OptionValueFormat(3, nameof(Uint));

        public static readonly OptionValueFormat String = new OptionValueFormat(4, nameof(String));

        public OptionValueFormat(int id, string name)
            : base(id, name)
        {
        }
    }


    //public readonly struct OptionDelta4Bit
    //{
    //    private const byte MASK = 0xF0;
    //    private readonly byte value;

    //    private OptionDelta4Bit(byte value)
    //    {
    //        var alignedValue = (UInt4)((value & MASK) >> 4);
    //        this.value = alignedValue;
    //    }

    //    public static explicit operator OptionDelta4Bit(byte value) => new OptionDelta4Bit(value);

    //    public static implicit operator UInt4(OptionDelta4Bit delta) => (UInt4)delta.value;

    //    public static implicit operator byte(OptionDelta4Bit delta) => delta.value;
    //}

    public readonly struct OptionDelta8Bit
    {
        private const byte MASK = 0xF0;
        private readonly byte value;

        private OptionDelta8Bit(byte value)
        {
            var alignedValue = (UInt4)((value & MASK) >> 4);
            this.value = alignedValue;
        }

        public static explicit operator OptionDelta4Bit(byte value) => new OptionDelta4Bit(value);

        public static implicit operator UInt4(OptionDelta4Bit delta) => (UInt4)delta.value;

        public static implicit operator byte(OptionDelta4Bit delta) => delta.value;
    }

    public class OptionLength
    {
        private const byte MASK = 0x0F;
        private readonly UInt4 value;

        private OptionLength(byte value)
        {
            var alignedValue = (UInt4)(value & MASK);
            this.value = alignedValue;
        }

        public static explicit operator OptionLength(byte value) => new OptionLength(value);

        public static implicit operator UInt4(OptionLength length) => length.value;

        public static implicit operator byte(OptionLength length) => length.value;
    }

    public readonly struct UInt2
    {
        private const byte MAX_VALUE = 0x03;
        private const byte MIN_VALUE = 0x00;

        private readonly byte value;

        private UInt2(byte value)
        {
            if (MAX_VALUE < value)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"The given value is out of the allowed range [{MIN_VALUE} - {MAX_VALUE}].");
            }

            this.value = value;
        }

        public static explicit operator UInt2(byte value) => new UInt2(value);

        public static implicit operator byte(UInt2 self) => self.value;

        public override string ToString()
        {
            return $"{this.value}";
        }
    }

    public readonly struct UInt4
    {
        private const byte MAX_VALUE = 0x0F;
        private const byte MIN_VALUE = 0x00;

        private readonly byte value;

        private UInt4(byte value)
        {
            if (MAX_VALUE < value)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"The given value is out of the allowed range [{MIN_VALUE} - {MAX_VALUE}].");
            }

            this.value = value;
        }

        public static explicit operator UInt4(byte value) => new UInt4(value);

        public static implicit operator byte(UInt4 self) => self.value;

        public override string ToString()
        {
            return $"{this.value}";
        }
    }

    /// <summary>
    /// Represents the version for a <see cref="CoapMessage"/>.
    /// </summary>
    public readonly struct Version
    {
        private const byte MASK = 0xC0;
        private readonly byte value;

        private Version(byte value)
        {
            this.value = (UInt2)((value & MASK) >> 6);
        }

        /// <summary>
        /// Gets the value of the <see cref="Version"/>.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public UInt2 Value => (UInt2)((this.value & MASK) >> 6);

        public static explicit operator Version(byte value) => new Version(value);

        public static implicit operator byte(Version version) => (byte)(version.value << 6);
    }

    public readonly struct Type
    {
        private const byte MASK = 0x30;
        private readonly byte value;

        private Type(byte value)
        {
            var alignedValue = (UInt2)((value & MASK) >> 4);
            this.value = alignedValue;
        }

        public UInt2 Value => (UInt2)((value & MASK) >> 4);

        public static explicit operator Type(byte value) => new Type(value);

        public static implicit operator byte(Type type) => (byte)(type.value << 4);

        public static Type Confirmable => (Type)0x00;

        public static Type NonConfirmable => (Type)(0x01 << 4);

        public static Type Acknowledgement => (Type)(0x02 << 4);

        public static Type Reset => (Type)(0x03 << 4);
    }

    public readonly struct TokenLength
    {
        private const byte MASK = 0x0F;
        private readonly byte value;

        private TokenLength(byte value)
        {
            var alignedValue = (UInt4)(value & MASK);
            if (alignedValue > 8)
            {
                throw new FormatMessageException();
            }

            this.value = alignedValue;
        }

        public UInt4 Value => (UInt4)(this.value & MASK);

        public static explicit operator TokenLength(byte value) => new TokenLength(value);

        public static implicit operator byte(TokenLength length) => length.Value;
    }

    public readonly struct UInt3
    {
        private const byte MAX_VALUE = 0x07;
        private const byte MIN_VALUE = 0x00;

        private readonly byte value;

        private UInt3(byte value)
        {
            if (value > MAX_VALUE)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.value = value;
        }

        public static explicit operator UInt3(byte value) => new UInt3(value);

        public static implicit operator byte(UInt3 self) => self.value;

        public override string ToString()
        {
            return $"{this.value}";
        }
    }

    public readonly struct UInt5
    {
        private const byte MAX_VALUE = 0x1F;
        private const byte MIN_VALUE = 0x00;

        private readonly byte value;

        private UInt5(byte value)
        {
            if (value > MAX_VALUE)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.value = value;
        }

        public static explicit operator UInt5(byte value) => new UInt5(value);

        public static implicit operator byte(UInt5 self) => self.value;

        public override string ToString()
        {
            return $"{this.value}";
        }
    }

    public class CodeClass : IEquatable<CodeClass>
    {
        private const byte CLASS_MASK = 0xE0;
        private readonly UInt3 value;

        private CodeClass(byte value)
        {
            this.value = (UInt3)((value & CLASS_MASK) >> 5);
        }

        public static explicit operator CodeClass(byte value) => new CodeClass(value);

        public static implicit operator byte(CodeClass @class) => @class.value;

        public bool Equals(CodeClass other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return value.Equals(other.value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((CodeClass)obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }

    public class CodeDetail : IEquatable<CodeDetail>
    {
        private const byte DETAIL_MASK = 0x1F;
        private readonly UInt5 value;

        private CodeDetail(byte value)
        {
            this.value = (UInt5)(value & DETAIL_MASK);
        }

        public static explicit operator CodeDetail(byte value) => new CodeDetail(value);

        public static implicit operator byte(CodeDetail @class) => @class.value;

        public bool Equals(CodeDetail other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return value.Equals(other.value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((CodeDetail)obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }

    public sealed class RequestCode : Code
    {
        public static readonly RequestCode EmptyMessage = new RequestCode(0, nameof(EmptyMessage), (CodeDetail)0);
        public static readonly RequestCode Get = new RequestCode(1, nameof(Get), (CodeDetail)1);
        public static readonly RequestCode Post = new RequestCode(2, nameof(Post), (CodeDetail)2);
        public static readonly RequestCode Put = new RequestCode(3, nameof(Put), (CodeDetail)3);
        public static readonly RequestCode Delete = new RequestCode(4, nameof(Delete), (CodeDetail)4);

        private RequestCode(int id, string name, CodeDetail detail)
            : base(id, name, (CodeClass)0x00, detail)
        {
        }
    }

    public sealed class SuccessResponse : Code
    {
        public static readonly SuccessResponse Created = new SuccessResponse(5, nameof(Created), (CodeDetail)1);
        public static readonly SuccessResponse Deleted = new SuccessResponse(6, nameof(Deleted), (CodeDetail)2);
        public static readonly SuccessResponse Valid = new SuccessResponse(7, nameof(Valid), (CodeDetail)3);
        public static readonly SuccessResponse Changed = new SuccessResponse(8, nameof(Changed), (CodeDetail)4);
        public static readonly SuccessResponse Content = new SuccessResponse(9, nameof(Content), (CodeDetail)5);

        private SuccessResponse(int id, string name, CodeDetail detail)
            : base(id, name, (CodeClass)0x40, detail)
        {
        }
    }

    public sealed class ClientErrorResponse : Code
    {
        public static readonly ClientErrorResponse BadRequest = new ClientErrorResponse(10, nameof(BadRequest), (CodeDetail)0);
        public static readonly ClientErrorResponse Unauthorized = new ClientErrorResponse(11, nameof(Unauthorized), (CodeDetail)1);
        public static readonly ClientErrorResponse BadOption = new ClientErrorResponse(12, nameof(BadOption), (CodeDetail)2);
        public static readonly ClientErrorResponse Forbidden = new ClientErrorResponse(13, nameof(Forbidden), (CodeDetail)3);
        public static readonly ClientErrorResponse NotFound = new ClientErrorResponse(14, nameof(NotFound), (CodeDetail)4);
        public static readonly ClientErrorResponse MethodNotAllowed = new ClientErrorResponse(15, nameof(MethodNotAllowed), (CodeDetail)5);
        public static readonly ClientErrorResponse NotAcceptable = new ClientErrorResponse(16, nameof(NotAcceptable), (CodeDetail)6);
        public static readonly ClientErrorResponse PreconditionFailed = new ClientErrorResponse(17, nameof(PreconditionFailed), (CodeDetail)12);
        public static readonly ClientErrorResponse RequestEntityTooLarge = new ClientErrorResponse(18, nameof(RequestEntityTooLarge), (CodeDetail)13);
        public static readonly ClientErrorResponse UnsupportedContentFormat = new ClientErrorResponse(19, nameof(UnsupportedContentFormat), (CodeDetail)15);

        private ClientErrorResponse(int id, string name, CodeDetail detail)
            : base(id, name, (CodeClass)0x80, detail)
        {
        }
    }

    public sealed class ServerErrorResponse : Code
    {
        public static readonly ServerErrorResponse InternalServerError = new ServerErrorResponse(20, nameof(InternalServerError), (CodeDetail)0);
        public static readonly ServerErrorResponse NotImplemented = new ServerErrorResponse(21, nameof(NotImplemented), (CodeDetail)1);
        public static readonly ServerErrorResponse BadGateway = new ServerErrorResponse(22, nameof(BadGateway), (CodeDetail)2);
        public static readonly ServerErrorResponse ServiceUnavailable = new ServerErrorResponse(23, nameof(ServiceUnavailable), (CodeDetail)3);
        public static readonly ServerErrorResponse GatewayTimeout = new ServerErrorResponse(24, nameof(GatewayTimeout), (CodeDetail)4);
        public static readonly ServerErrorResponse ProxyingNotSupported = new ServerErrorResponse(25, nameof(ProxyingNotSupported), (CodeDetail)5);

        private ServerErrorResponse(int id, string name, CodeDetail detail)
            : base(id, name, (CodeClass)0xA0, detail)
        {
        }
    }

    public readonly struct MessageId
    {
        private readonly UInt16 value;

        private MessageId(ReadOnlySpan<byte> value)
        {
            if (value.Length != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Expected two bytes, found {value.Length} bytes.");
            }

            this.value = MemoryMarshal.Read<UInt16>(value);
        }

        public UInt16 Value => (UInt16)IPAddress.HostToNetworkOrder((short)this.value);

        public static explicit operator MessageId(byte[] value) => new MessageId(value.AsSpan());

        public static explicit operator MessageId(ushort value) => new MessageId(BitConverter.GetBytes(value));

        public static implicit operator byte[](MessageId id) => BitConverter.GetBytes(id.Value);

        public override string ToString()
        {
            return this.value.ToString();
        }
    }

    public class Token
    {
        private readonly byte value;

        private Token(byte value)
        {
            this.value = value;
        }

        public static explicit operator Token(byte value) => new Token(value);

        public static implicit operator byte(Token token) => token.value;

        public override string ToString()
        {
            return this.value.ToString();
        }

        public static IEnumerable<Token> Parse(ReadOnlySpan<byte> bytes)
        {
            var tokens = new List<Token>();
            foreach (var b in bytes)
            {

                tokens.Add((Token)b);
            }

            return tokens;
        }
    }

    /// <summary>
    /// Represents the common base class for <see cref="CoapMessage"/> codes.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.Net.Enumeration" />
    public abstract class Code : Enumeration
    {
        private static readonly List<CodeClass> AllowedClasses = new List<CodeClass>()
        {
            (CodeClass)0x00,
            (CodeClass)0x02,
            (CodeClass)0x04,
            (CodeClass)0x05,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Code"/> class.
        /// </summary>
        /// <param name="id">The identifier to identify the <see cref="Code"/> in the enumeration.</param>
        /// <param name="name">The name of the <see cref="Code"/>.</param>
        /// <param name="class">The class value of the <see cref="Code"/>.</param>
        /// <param name="detail">The detail value of the <see cref="Code"/>.</param>
        protected Code(int id, string name, CodeClass @class, CodeDetail detail)
            : base(id, name)
        {
            this.Class = @class;
            this.Detail = detail;
        }

        /// <summary>
        /// Gets the class value of the <see cref="Code"/>.
        /// </summary>
        /// <value>
        /// The class.
        /// </value>
        public CodeClass Class { get; }

        /// <summary>
        /// Gets the detail value of the <see cref="Code"/>.
        /// </summary>
        /// <value>
        /// The detail.
        /// </value>
        public CodeDetail Detail { get; }

        public static implicit operator byte(Code code) => (byte)(code.Class | code.Detail);

        /// <summary>
        /// Parses the specified <see cref="byte"/> to get the equivalent <see cref="Code"/>.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> that represents a <see cref="Code"/>.</param>
        /// <returns>The parsed <see cref="Code"/> from the given <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException">A reserved class value was given.</exception>
        public static Code Parse(byte value)
        {
            var @class = (CodeClass)value;
            if (!Code.AllowedClasses.Contains(@class))
            {
                throw new ArgumentException("Reserved class value", nameof(@class));
            }

            var detail = (CodeDetail)value;
            var code = Code.GetCode(@class, detail);
            return code;
        }

        /// <summary>
        /// Gets the <see cref="Code"/> based on the given <see cref="CodeClass"/> and <see cref="CodeDetail"/>.
        /// </summary>
        /// <param name="class">The class value of the wanted <see cref="Code"/>.</param>
        /// <param name="detail">The detail value of the wanted <see cref="Code"/>.</param>
        /// <returns>The <see cref="Code"/> with the given <paramref name="class"/> and <paramref name="detail"/>.</returns>
        public static Code GetCode(CodeClass @class, CodeDetail detail)
        {
            var codes = Code.GetAll();
            var code = codes.Single(c => c.Class.Equals(@class) && c.Detail.Equals(detail));
            return code;
        }

        /// <summary>
        /// Gets the <see cref="Code"/> based on the given name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Code"/>.</param>
        /// <returns>The <see cref="Code"/> with the given <paramref name="name"/>.</returns>
        public static Code GetCode(string name)
        {
            var code = Code.Get(name);
            return code;
        }

        public override string ToString() => this.Name.ToUpperInvariant();

        private static IEnumerable<Code> GetAll()
        {
            var items = typeof(Code)
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Code)) && !t.IsAbstract)
                .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
                .Select(f => f.GetValue(null))
                .Cast<Code>();

            return items;
        }

        private static Code Get(string name)
        {
            var codes = Code.GetAll().ToList();
            if (!codes.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new KeyNotFoundException($"Does not found {nameof(Code)} with name {name}.");
            }

            var code = codes.Single(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return code;
        }
    }

    public abstract class Enumeration : IComparable
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => this.Name;

        /// <summary>
        /// Gets all items with of the given enumeration with type <typeparamref name="TEnumeration"/>.
        /// </summary>
        /// <typeparam name="TEnumeration">The type of the base.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> with items of type <typeparamref name="TEnumeration"/>. The <see cref="IEnumerable{T}"/>
        /// contains all items in that enumeration.</returns>
        public static IEnumerable<TEnumeration> GetAll<TEnumeration>()
            where TEnumeration : Enumeration
        {
            var items = typeof(TEnumeration)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<TEnumeration>();
            return items;
        }

        /// <summary>
        /// Gets the item out of the enumeration with the specified name.
        /// </summary>
        /// <typeparam name="TEnumeration">The type of the enumeration.</typeparam>
        /// <param name="name">The name of the searching item.</param>
        /// <returns>The item with the given <paramref name="name"/>.</returns>
        /// <exception cref="KeyNotFoundException">If no item was found with the <paramref name="name"/> in the enumeration of the given <typeparamref name="TEnumeration"/>.</exception>
        public static TEnumeration Get<TEnumeration>(string name)
            where TEnumeration : Enumeration
        {
            var items = Enumeration.GetAll<TEnumeration>().ToList();
            if (!items.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new KeyNotFoundException($"Does not found an item with the name {name} in the enumeration of {nameof(TEnumeration)}.");
            }

            var item = items.Single(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return item;
        }

        /// <summary>
        /// Gets the item out of the enumeration with the specified identifier.
        /// </summary>
        /// <typeparam name="TEnumeration">The type of the enumeration.</typeparam>
        /// <param name="id">The identifier of the searching item.</param>
        /// <returns>The item with the given <paramref name="id"/>.</returns>
        /// <exception cref="KeyNotFoundException">If not item was found with the <paramref name="id"/> in the enumeration of the given <typeparamref name="TEnumeration"/>.</exception>
        public static TEnumeration Get<TEnumeration>(int id)
            where TEnumeration : Enumeration
        {
            var items = Enumeration.GetAll<TEnumeration>().ToList();
            if (!items.Any(i => i.Id.Equals(id)))
            {
                throw new KeyNotFoundException($"Does not found an item with the id {id} in the enumeration of {nameof(TEnumeration)}.");
            }

            var item = items.Single(i => i.Id.Equals(id));
            return item;
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
    }

    public class FormatMessageException : ApplicationException
    {

    }

    public class UdpDatagram
    {
        public UdpDatagram(UdpReceiveResult result)
        {
            this.Result = result;
        }

        public UdpReceiveResult Result { get; }
    }
}
