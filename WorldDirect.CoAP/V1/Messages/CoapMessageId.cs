namespace WorldDirect.CoAP.V1.Messages
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public struct CoapMessageId : IEquatable<CoapMessageId>
    {
        /// <summary>
        /// The default value of a <see cref="CoapMessageId"/>. It represents a <see cref="CoapMessageId"/> with value 0.
        /// </summary>
        public static readonly CoapMessageId Default = new CoapMessageId(0);

        private static readonly Random Random = new Random();
        private readonly ushort value;

        public CoapMessageId(ushort value)
        {
            this.value = value;
        }

        public static explicit operator CoapMessageId(ushort value) => new CoapMessageId(value);

        public static implicit operator ushort(CoapMessageId messageId) => messageId.value;

        public static bool operator ==(CoapMessageId left, CoapMessageId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CoapMessageId left, CoapMessageId right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapMessageId"/> structure.
        /// </summary>
        /// <returns>The new randomly generated <see cref="CoapMessageId"/>.</returns>
        public static CoapMessageId NewMessageId()
        {
            var value = (ushort)Random.Next(ushort.MinValue, ushort.MaxValue + 1);
            var messageId = new CoapMessageId(value);
            return messageId;
        }

        public bool Equals(CoapMessageId other)
        {
            return this.value == other.value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((CoapMessageId)obj);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public override string ToString()
        {
            return this.value.ToString("D");
        }
    }
}
