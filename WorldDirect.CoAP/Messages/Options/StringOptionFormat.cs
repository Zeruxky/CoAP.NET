namespace WorldDirect.CoAP.Messages.Options
{
    using System;
    using System.Text;

    public abstract class StringOptionFormat : ICoapOption, IEquatable<StringOptionFormat>
    {
        private readonly Encoding encoding;

        protected StringOptionFormat(byte[] value, Encoding encoding)
        {
            this.RawValue = value;
            this.encoding = encoding;
        }

        protected StringOptionFormat(byte[] value)
            : this(Encoding.UTF8.GetString(value))
        {
        }

        protected StringOptionFormat(string value, Encoding encoding)
            : this(encoding.GetBytes(value), encoding)
        {
        }

        protected StringOptionFormat(string value)
            : this(value, Encoding.UTF8)
        {
        }

        public abstract ushort Number { get; }

        public byte[] RawValue { get; }

        public string Value => this.encoding.GetString(this.RawValue);

        public static bool operator ==(StringOptionFormat left, StringOptionFormat right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StringOptionFormat left, StringOptionFormat right)
        {
            return !Equals(left, right);
        }

        public bool Equals(StringOptionFormat other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Number.Equals(other.Number) && this.Value.Equals(other.Value);
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

            return this.Equals((StringOptionFormat)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.encoding, this.Number, this.RawValue);
        }
    }
}
