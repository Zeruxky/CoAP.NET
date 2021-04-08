namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Buffers;
    using System.Linq;

    /// <summary>
    /// Represents a non-generic option specified by RFC 7252.
    /// </summary>
    public abstract class CoapOption : IEquatable<CoapOption>
    {
        private const ushort MIN_LENGTH = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapOption"/> class.
        /// </summary>
        /// <param name="number">The number of that <see cref="CoapOption"/>.</param>
        /// <param name="minLength">The minimum allowed length for that <see cref="CoapOption"/>.</param>
        /// <param name="maxLength">The maximum allowed length for that <see cref="CoapOption"/>.</param>
        /// <param name="isRepeatable">If set to <see langword="true"/> the <see cref="CoapOption"/> can be appear
        /// multiple times in a <see cref="OptionCollection"/>; Otherwise <see langword="false"/>.</param>
        /// <exception cref="ArgumentException">Throws if the <paramref name="minLength"/> is greater than the <paramref name="maxLength"/>.</exception>
        protected CoapOption(ushort number, uint minLength, uint maxLength, bool isRepeatable)
        {
            this.Number = number;
            this.Name = this.GetOptionName();

            if (minLength > maxLength)
            {
                throw new ArgumentException($"The minimum length of {minLength} can not be greater than the maximum length of {maxLength}.");
            }

            this.MinLength = minLength;
            this.MaxLength = maxLength;
            this.IsRepeatable = isRepeatable;
            this.IsCritical = Convert.ToBoolean(this.Number & 1);
            this.IsElective = !this.IsCritical;
            this.IsUnsafe = Convert.ToBoolean(this.Number & 2);
            this.IsSafeToForward = !this.IsUnsafe;
            this.IsNoCacheKey = (this.Number & 0x1e) == 0x1c;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapOption"/> class.
        /// </summary>
        /// <param name="number">The number of that <see cref="CoapOption"/>.</param>
        /// <param name="maxLength">The maximum allowed length for that <see cref="CoapOption"/>.</param>
        /// <param name="isRepeatable">If set to <see langword="true"/> the <see cref="CoapOption"/> can be appear
        /// multiple times in a <see cref="OptionCollection"/>; Otherwise <see langword="false"/>.</param>
        protected CoapOption(ushort number, uint maxLength, bool isRepeatable)
            : this(number, MIN_LENGTH, maxLength, isRepeatable)
        {
        }

        /// <summary>
        /// Gets the number of that <see cref="CoapOption"/>.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public ushort Number { get; }

        /// <summary>
        /// Gets the name of that <see cref="CoapOption"/>.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the maximum allowed length in bytes for that <see cref="CoapOption"/>.
        /// </summary>
        /// <value>
        /// The maximum allowed length in bytes.
        /// </value>
        public uint MaxLength { get; }

        /// <summary>
        /// Gets the minimum allowed length in bytes for that <see cref="CoapOption"/>.
        /// </summary>
        /// <value>
        /// The minimum allowed length in bytes.
        /// </value>
        public uint MinLength { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is elective.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is elective; otherwise, <c>false</c>.
        /// </value>
        public bool IsElective { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is safe to forward.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is safe to forward; otherwise, <c>false</c>.
        /// </value>
        public bool IsSafeToForward { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is critical.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is critical; otherwise, <c>false</c>.
        /// </value>
        public bool IsCritical { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is unsafe.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is unsafe; otherwise, <c>false</c>.
        /// </value>
        public bool IsUnsafe { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is no cache key.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is no cache key; otherwise, <c>false</c>.
        /// </value>
        public bool IsNoCacheKey { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is repeatable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is repeatable; otherwise, <c>false</c>.
        /// </value>
        public bool IsRepeatable { get; }

        public static bool operator ==(CoapOption left, CoapOption right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CoapOption left, CoapOption right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string ToString() => $"{this.Name} ({this.Number})";

        /// <inheritdoc />
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

            return this.Number.Equals(other.Number) && this.Name.Equals(other.Name);
        }

        /// <inheritdoc />
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

            return this.Equals((CoapOption)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Number, this.Name);
        }

        private string GetOptionName()
        {
            // Special case for the ETag option, which will not be dasherized.
            if (this.GetType() == typeof(ETag))
            {
                return "ETag";
            }

            return this.GetType().Name.Dasherize();
        }
    }
}
