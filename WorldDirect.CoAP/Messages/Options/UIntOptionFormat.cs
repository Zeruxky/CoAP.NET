namespace WorldDirect.CoAP.Messages.Options
{
    using System;
    using System.Buffers.Binary;

    public abstract class UIntOptionFormat : ICoapOption, IEquatable<UIntOptionFormat>
    {
        protected UIntOptionFormat(byte[] value)
        {
            this.RawValue = value;
        }

        protected UIntOptionFormat(uint value)
            : this(BitConverter.GetBytes(value))
        {
        }

        public abstract ushort Number { get; }

        public byte[] RawValue { get; }

        public uint Value => BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt32(this.RawValue, 0));

        public static bool operator ==(UIntOptionFormat left, UIntOptionFormat right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UIntOptionFormat left, UIntOptionFormat right)
        {
            return !Equals(left, right);
        }

        public bool Equals(UIntOptionFormat other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Number == other.Number && this.Value.Equals(other.Value);
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

            return this.Equals((UIntOptionFormat)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Number, this.RawValue);
        }
    }
}
