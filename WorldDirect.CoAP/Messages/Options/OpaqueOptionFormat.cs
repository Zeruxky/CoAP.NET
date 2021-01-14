namespace WorldDirect.CoAP.Messages.Options
{
    using System;
    using System.Linq;

    public abstract class OpaqueOptionFormat : ICoapOption, IEquatable<OpaqueOptionFormat>
    {
        protected OpaqueOptionFormat(byte[] value)
        {
            this.RawValue = value;
        }

        public abstract ushort Number { get; }

        public byte[] RawValue { get; }

        public static bool operator ==(OpaqueOptionFormat left, OpaqueOptionFormat right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OpaqueOptionFormat left, OpaqueOptionFormat right)
        {
            return !Equals(left, right);
        }

        public bool Equals(OpaqueOptionFormat other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Number == other.Number && this.RawValue.SequenceEqual(other.RawValue);
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

            return this.Equals((OpaqueOptionFormat)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Number, this.RawValue);
        }
    }
}
