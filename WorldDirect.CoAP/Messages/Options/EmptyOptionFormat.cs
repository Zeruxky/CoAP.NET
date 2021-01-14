namespace WorldDirect.CoAP.Messages.Options
{
    using System;

    public abstract class EmptyOptionFormat : ICoapOption, IEquatable<EmptyOptionFormat>
    {
        protected EmptyOptionFormat()
        {
            this.RawValue = new byte[0];
        }

        public abstract ushort Number { get; }

        public byte[] RawValue { get; }

        public static bool operator ==(EmptyOptionFormat left, EmptyOptionFormat right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EmptyOptionFormat left, EmptyOptionFormat right)
        {
            return !Equals(left, right);
        }

        public bool Equals(EmptyOptionFormat other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Number == other.Number;
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

            return this.Equals((EmptyOptionFormat) obj);
        }

        public override int GetHashCode()
        {
            return this.Number.GetHashCode();
        }
    }
}
