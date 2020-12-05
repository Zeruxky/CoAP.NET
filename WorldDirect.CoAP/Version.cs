// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    /// <summary>
    /// Represents the first two bits unsigned integer of a CoAP message.
    /// It indicates the CoAP version number.
    /// </summary>
    /// <remarks>
    /// In this implementation (RFC 7252) the default value of the version is 1 (01 in binary).
    /// Other values are reserved for future versions. Messages with unknown version numbers
    /// will be silently ignored.
    /// </remarks>
    public class Version
    {
        /// <summary>
        /// The default value for the <see cref="Version"/> specified in RFC 7252.
        /// </summary>
        public static readonly Version V1 = new Version((UInt2)1);

        private readonly UInt2 value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Version"/> class.
        /// </summary>
        /// <param name="value">The <see cref="UInt2"/> value that represents the version number of a CoAP Message.</param>
        public Version(UInt2 value)
        {
            this.value = value;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="UInt2"/> to <see cref="Version"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A <see cref="Version"/> that is equivalent to the given <see cref="UInt2"/> value.
        /// </returns>
        public static explicit operator Version(UInt2 value) => new Version(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Version"/> to <see cref="UInt2"/>.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// A <see cref="UInt2"/> that is equivalent to the given <see cref="Version"/> value.
        /// </returns>
        public static implicit operator UInt2(Version version) => version.value;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if both <see cref="Version"/>s are equal, otherwise <c>false</c>.
        /// </returns>
        public static bool operator ==(Version left, Version right) => Equals(left, right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if both <see cref="Version"/>s are not equal, otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(Version left, Version right) => !Equals(left, right);

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

            return this.Equals((Version)obj);
        }

        /// <summary>
        /// Indicates if the specified <see cref="Version"/> is equal to the current <see cref="Version"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Version"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="other"/> <see cref="Version"/> is equal to the current <see cref="Version"/>.</returns>
        public bool Equals(Version other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.value == other.value;
        }

        /// <inheritdoc />
        public override int GetHashCode() => this.value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => this.value.ToString();
    }
}
