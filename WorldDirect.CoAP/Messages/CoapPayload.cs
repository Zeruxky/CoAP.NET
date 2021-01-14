namespace WorldDirect.CoAP.Messages
{
    using System;
    using System.Linq;
    using System.Text;

    public readonly struct CoapPayload : IEquatable<CoapPayload>
    {
        public static readonly CoapPayload EmptyPayload = new CoapPayload(new byte[0]);

        private readonly byte[] value;

        public CoapPayload(Span<byte> value)
        {
            this.value = value.ToArray();
        }

        public static bool operator ==(CoapPayload left, CoapPayload right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CoapPayload left, CoapPayload right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(this.value);
        }

        public bool Equals(CoapPayload other)
        {
            return this.value.SequenceEqual(other.value);
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

            return this.Equals((CoapPayload)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.value);
        }
    }
}