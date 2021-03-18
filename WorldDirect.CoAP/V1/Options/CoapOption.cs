namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Linq;
    using Common.Extensions;

    /// <summary>
    /// Represents a option specified by RFC 7252.
    /// </summary>
    public abstract class CoapOption : IEquatable<CoapOption>
    {
        private const ushort MIN_LENGTH = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapOption"/> class.
        /// </summary>
        /// <param name="value">The byte array (in network byte order) that represents the value of that option.</param>
        /// <remarks>
        /// If the system's computer architecture is in little endian order, the <paramref name="value"/>
        /// will be reversed, because the <paramref name="value"/> is expected to be in network byte order (big endian order).
        /// </remarks>
        protected CoapOption(ushort number, byte[] value,  uint maxLength, uint minLength)
        {
            this.Number = number;
            this.Name = this.Dasherize();

            if (minLength > maxLength)
            {
                throw new ArgumentException($"The minimum length of {minLength} can not be greater than the maximum length of {maxLength}.");
            }

            this.MinLength = minLength;
            this.MaxLength = maxLength;

            if (value.Length < minLength || value.Length > maxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(this.Value), this.Value, $"The length of the value is out of range [{this.MinLength} - {this.MaxLength} bytes].");
            }

            this.Value = value;
        }

        protected CoapOption(ushort number, byte[] value, uint maxLength)
            : this(number, value, maxLength, MIN_LENGTH)
        {
        }

        /// <summary>
        /// Gets the number of that <see cref="CoapOption"/>.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public ushort Number { get; }

        public string Name { get; }

        /// <summary>
        /// Gets the raw value of that <see cref="CoapOption"/> in the system's computer architecture.
        /// </summary>
        /// <value>
        /// The raw value.
        /// </value>
        public byte[] Value { get; }

        public uint MaxLength { get; }

        public uint MinLength { get; }

        public static bool operator ==(CoapOption left, CoapOption right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CoapOption left, CoapOption right)
        {
            return !Equals(left, right);
        }

        public override string ToString() => $"{this.Name} ({this.Number})";

        public bool Equals(CoapOption other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Number.Equals(other.Number) && this.Name.Equals(other.Name) && this.Value.SequenceEqual(other.Value);
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

            return this.Equals((CoapOption) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Number, this.Name, this.Value);
        }
    }
}
