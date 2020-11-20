namespace WorldDirect.CoAP.Net
{
    using System;
    using System.Buffers;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public class CoapClient
    {
        private readonly UdpClient listener;

        public CoapClient(string hostname, int port)
        {
            this.listener = new UdpClient(hostname, port);
        }

        public CoapClient(IPEndPoint endpoint)
        {
            this.listener = new UdpClient(endpoint);
        }

        public async Task<CoapMessage> SendAsync(CoapRequestMessage message, CancellationToken ct)
        {
            var content = message.GetBytes().ToArray();
            var sendBytes = await this.listener.SendAsync(content, content.Length).ConfigureAwait(false);
            if (sendBytes < 1)
            {
                throw new ArgumentException($"No bytes sent to {this.listener.Client.RemoteEndPoint}.", nameof(sendBytes));
            }

            var result = await this.listener.ReceiveAsync().ConfigureAwait(false);
            var diagram = new UdpDatagram(result);
            var response = new CoapRequestMessage(diagram);

            return response;
        }
    }

    public class CoapHeader
    {
        public CoapHeader(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length != 4)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.Version = (Net.Version)bytes[0];
            this.Type = (Net.Type)bytes[0];
            this.TokenLength = (Net.TokenLength)bytes[0];
            this.Code = Net.Code.Parse(bytes[1]);
            this.MessageId = (Net.MessageId)new[] {bytes[2], bytes[3]};
        }

        public Version Version { get; }

        public Type Type { get; }

        public TokenLength TokenLength { get; }

        public Code Code { get; }

        public MessageId MessageId { get; }

        public IEnumerable<byte> GetBytes()
        {
            yield return this.Version;
            yield return this.Type;
            yield return this.TokenLength;
            yield return this.Code;
            foreach (var b in (byte[])this.MessageId)
            {
                yield return b;
            }
        }
    }

    public class CoapRequestMessage : CoapMessage
    {
        public CoapRequestMessage(UdpDatagram datagram)
            : base(datagram)
        {
        }
    }

    public abstract class CoapMessage : IDisposable
    {
        private readonly IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(5);
        private readonly Memory<byte> memory;
        private readonly IEnumerable<Token> tokens;
        private readonly IEnumerable<Option> options;

        protected CoapMessage(UdpDatagram datagram)
        {
            this.memory = this.owner.Memory;
            datagram.Result.Buffer.CopyTo(this.memory);
            this.Header = new CoapHeader(this.memory.Span.Slice(0, 4));
            this.tokens = Token.Parse(this.memory.Span.Slice(4, this.Header.TokenLength));
        }

        public CoapHeader Header { get; }

        public IReadOnlyList<Token> Tokens => this.tokens.ToList();

        public IReadOnlyList<Option> Options { get; }

        public byte[] Payload { get; }

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

        public void Dispose()
        {
            owner?.Dispose();
        }
    }

    public class Option
    {
        private readonly byte[] value;

        private Option(ReadOnlySpan<byte> bytes)
        {
            this.Delta = (OptionDelta)bytes[0];
            this.Length = (OptionLength)bytes[0];
        }

        public OptionDelta Delta { get; }

        public OptionLength Length { get; }

        public OptionValueFormat Format { get; }

        public byte[] Value { get; }

        public IEnumerable<byte> GetBytes()
        {
            yield return this.Delta;
            yield return this.Length;
        }
    }

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

    public class OptionDelta
    {
        private const byte MASK = 0xF0;
        private readonly UInt4 value;

        private OptionDelta(byte value)
        {
            var alignedValue = (UInt4)((value & MASK) >> 4);
            this.value = alignedValue;
        }

        public static explicit operator OptionDelta(byte value) => new OptionDelta(value);

        public static implicit operator UInt4(OptionDelta delta) => delta.value;

        public static implicit operator byte(OptionDelta delta) => delta.value;
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

    public class Version
    {
        private const byte MASK = 0xC0;
        private readonly UInt2 value;

        private Version(byte value)
        {
            var alignedValue = (UInt2)((value & MASK) >> 6);
            if (!this.AllowedVersions.Contains(alignedValue))
            {
                throw new ArgumentException("Unsupported version found.");
            }

            this.value = alignedValue;
        }

        public static explicit operator Version(byte value) => new Version(value);

        public static implicit operator byte(Version version) => (byte)(version.value << 6);

        public IReadOnlyCollection<byte> AllowedVersions => new List<byte>()
        {
            0x01,
        };
    }

    public class Type
    {
        private const byte MASK = 0x30;
        private readonly UInt2 value;

        private Type(byte value)
        {
            var alignedValue = (UInt2)((value & MASK) >> 4);
            this.value = alignedValue;
        }

        public static explicit operator Type(byte value) => new Type(value);

        public static implicit operator byte(Type type) => (byte)(type.value << 4);

        public static Type Confirmable => (Type)0x00;

        public static Type NonConfirmable => (Type)(0x01 << 4);

        public static Type Acknowledgement => (Type)(0x02 << 4);

        public static Type Reset => (Type)(0x03 << 4);
    }

    public class TokenLength
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

        public static explicit operator TokenLength(byte value) => new TokenLength(value);

        public static implicit operator byte(TokenLength length) => length.value;

        public static implicit operator UInt4(TokenLength length) => (UInt4)length.value;
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

            return Equals((CodeClass) obj);
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

            return Equals((CodeDetail) obj);
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
        public static readonly SuccessResponse Created = new SuccessResponse(0, nameof(Created), (CodeDetail)1);
        public static readonly SuccessResponse Deleted = new SuccessResponse(1, nameof(Deleted), (CodeDetail)2);
        public static readonly SuccessResponse Valid = new SuccessResponse(2, nameof(Valid), (CodeDetail)3);
        public static readonly SuccessResponse Changed = new SuccessResponse(3, nameof(Changed), (CodeDetail)4);
        public static readonly SuccessResponse Content = new SuccessResponse(4, nameof(Content), (CodeDetail)5);


        private SuccessResponse(int id, string name, CodeDetail detail)
            : base(id, name, (CodeClass)0x40, detail)
        {
        }
    }

    public sealed class ClientErrorResponse : Code
    {
        public static readonly ClientErrorResponse BadRequest = new ClientErrorResponse(5, nameof(BadRequest), (CodeDetail)0);
        public static readonly ClientErrorResponse Unauthorized = new ClientErrorResponse(5, nameof(Unauthorized), (CodeDetail)1);
        public static readonly ClientErrorResponse BadOption = new ClientErrorResponse(5, nameof(BadOption), (CodeDetail)2);
        public static readonly ClientErrorResponse Forbidden = new ClientErrorResponse(5, nameof(Forbidden), (CodeDetail)3);
        public static readonly ClientErrorResponse NotFound = new ClientErrorResponse(5, nameof(NotFound), (CodeDetail)4);
        public static readonly ClientErrorResponse MethodNotAllowed = new ClientErrorResponse(5, nameof(MethodNotAllowed), (CodeDetail)5);
        public static readonly ClientErrorResponse NotAcceptable = new ClientErrorResponse(5, nameof(NotAcceptable), (CodeDetail)6);
        public static readonly ClientErrorResponse PreconditionFailed = new ClientErrorResponse(5, nameof(PreconditionFailed), (CodeDetail)12);
        public static readonly ClientErrorResponse RequestEntityTooLarge = new ClientErrorResponse(5, nameof(RequestEntityTooLarge), (CodeDetail)13);
        public static readonly ClientErrorResponse UnsupportedContentFormat = new ClientErrorResponse(5, nameof(UnsupportedContentFormat), (CodeDetail)15);

        private ClientErrorResponse(int id, string name, CodeDetail detail)
            : base(id, name, (CodeClass)0x80, detail)
        {
        }
    }

    public sealed class ServerErrorResponse : Code
    {
        public static readonly ServerErrorResponse InternalServerError = new ServerErrorResponse(15, nameof(InternalServerError), (CodeDetail)0);
        public static readonly ServerErrorResponse NotImplemented = new ServerErrorResponse(15, nameof(NotImplemented), (CodeDetail)1);
        public static readonly ServerErrorResponse BadGateway = new ServerErrorResponse(15, nameof(BadGateway), (CodeDetail)2);
        public static readonly ServerErrorResponse ServiceUnavailable = new ServerErrorResponse(15, nameof(ServiceUnavailable), (CodeDetail)3);
        public static readonly ServerErrorResponse GatewayTimeout = new ServerErrorResponse(15, nameof(GatewayTimeout), (CodeDetail)4);
        public static readonly ServerErrorResponse ProxyingNotSupported = new ServerErrorResponse(15, nameof(ProxyingNotSupported), (CodeDetail)5);

        private ServerErrorResponse(int id, string name, CodeDetail detail)
            : base(id, name, (CodeClass)0xA0, detail)
        {
        }
    }

    public class MessageId
    {
        private readonly UInt16 value;

        private MessageId(byte[] value)
        {
            if (value.Length != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Expected two bytes, found {value.Length} bytes.");
            }

            this.value = BitConverter.ToUInt16(value, 0);
        }

        public static explicit operator MessageId(byte[] value) => new MessageId(value);

        public static implicit operator byte[](MessageId id) => BitConverter.GetBytes(id.value);
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

    public abstract class Code : Enumeration
    {
        protected Code(int id, string name, CodeClass @class, CodeDetail detail)
            : base(id, name)
        {
            this.Class = @class;
            this.Detail = detail;
        }

        public CodeClass Class { get; }

        public CodeDetail Detail { get; }

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

        public static Code GetCode(CodeClass @class, CodeDetail detail)
        {
            var codes = Enumeration.GetAll<Code>();
            return codes.Single(c => c.Class.Equals(@class) && c.Detail.Equals(detail));
        }

        public static implicit operator byte(Code code) => (byte)(code.Class | code.Detail);

        public bool IsRequest => this.Class == 0;

        public bool IsResponse => this.Class != 0;

        public static IReadOnlyCollection<byte> AllowedClasses => new List<byte>()
        {
            0x00,
            0x02,
            0x04,
            0x05,
        };
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

        public override string ToString() => Name;

        //public static IEnumerable<T> GetAll<T>() where T : Enumeration
        //{
        //    var fields = typeof(T).GetFields(BindingFlags.Public |
        //                                     BindingFlags.Static |
        //                                     BindingFlags.DeclaredOnly);

        //    return fields.Select(f => f.GetValue(null)).Cast<T>();
        //}

        public static IEnumerable<TBase> GetAll<TBase>()
            where TBase : Enumeration
        {
            var fields = typeof(TBase)
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(TBase)) && !t.IsAbstract)
                .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
                .Select(f => f.GetValue(null)).Cast<TBase>();

            return fields;
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
