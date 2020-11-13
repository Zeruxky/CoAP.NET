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

        public async Task<CoapResponseMessage> SendAsync(CoapRequestMessage request, CancellationToken ct)
        {
            var content = request.Content.Value.ToArray();
            var sendBytes = await this.listener.SendAsync(content, content.Length).ConfigureAwait(false);
            if (sendBytes < 1)
            {
                throw new ArgumentException($"No bytes sent to {this.listener.Client.RemoteEndPoint}.", nameof(sendBytes));
            }

            var result = await this.listener.ReceiveAsync().ConfigureAwait(false);
            var response = new CoapResponseMessage(result.Buffer);
            return response;
        }
    }

    public class CoapMessage
    {

    }

    public class CoapRequestMessage
    {
        public CoapContent Content { get; }
    }

    public class CoapContent
    {
        private Memory<byte> value;

        public CoapContent(byte[] value)
        {
            this.value = new Memory<byte>(value);
        }

        public ReadOnlyMemory<byte> Value => this.value;
    }

    public class CoapResponseMessage
    {

        public CoapContent Content { get; }

        public CoapCode Code => CoapCode.Parse(this.Content.Value.Span.Slice(1, 1));

        public int Version
        {
            get
            {
                var x = this.Content.Value.Span.Slice(0, 1);
                var y = Encoding.UTF8.GetString(x.ToArray(), 0, 2);

                return 1;
            }
        }
    }

    public abstract class CoapCode
    {
        private readonly ushort @class;
        private readonly ushort detail;
        
        protected CoapCode(ushort @class, ushort detail)
        {
            this.@class = @class;
            this.detail = detail;
        }

        public static CoapCode Parse(string value)
        {
            ushort @class;
            try
            {
                @class = ushort.Parse(value.Substring(0, 1));
            }
            catch (OverflowException)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Class value of {value} is not allowed to be negative.");
            }

            if (@class > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(@class), @class, "Class value is only allowed to be between 0 and 7.");
            }
            
            ushort detail;
            try
            {
                detail = ushort.Parse(value.Substring(2, 2));
            }
            catch (OverflowException)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Detail value of {value} is not allowed to be negative.");
            }

            if (detail > 31)
            {
                throw new ArgumentOutOfRangeException(nameof(detail), detail, "Detail value is only allowed to be between 0 and 31.");
            }

            if (@class == 0)
            {
                return new RequestCode(detail);
            }

            if (@class >= 2 && @class <= 5)
            {
                return new ResponseCode(detail);
            }
        }

        public CoapCode Parse(ReadOnlySpan<byte> value)
        {

        }

        protected static void ValidateFormat(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value), "Value to parse is null or empty.");
            }

            if (value.Length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Expecting four characters, but found {value.Length} characters.");
            }

            if (!char.IsDigit(value[0]))
            {
                throw new FormatException("Expecting first character to be a digit.");
            }

            if (value[1] != '.')
            {
                throw new FormatException("Expecting second character to be a dot.");
            }

            if (!char.IsDigit(value[2]))
            {
                throw new FormatException("Expecting third character to be a digit.");
            }

            if (!char.IsDigit(value[3]))
            {
                throw new FormatException("Expecting last character to be a digit.");
            }
        }

        public ushort MinValue { get; protected set; }

        public ushort MaxValue { get; protected set; }
    }

    public class RequestCode : CoapCode
    {
        public RequestCode(ushort detailValue)
            : base(0, detailValue)
        {
            this.MinValue = 0;
            this.MaxValue = 0;
        }
    }


    public class ResponseCode : CoapCode
    {
        public ResponseCode(ushort @class, ushort detail)
            : base(@class, detail)
        {
            if (@class < ResponseCode.MinValue || @class > ResponseCode.MaxValue)
            {

            }
        }

        public static ushort MinValue => 2;

        public static ushort MaxValue => 5;
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

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
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
}
