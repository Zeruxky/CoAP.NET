// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a <see cref="CoapOption"/> as a opaque sequence of bytes. This means the value of that <see cref="CoapOption"/>
    /// is a opaque sequence of bytes.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.Messages.Options.CoapOption" />
    /// <seealso cref="System.IEquatable{WorldDirect.CoAP.Messages.Options.OpaqueOptionFormat}" />
    public abstract class OpaqueOptionFormat : CoapOption, IEquatable<OpaqueOptionFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpaqueOptionFormat"/> class.
        /// </summary>
        /// <param name="value">The byte array (in network byte order) that represents the value of that option.</param>
        /// <remarks>
        /// If the system's computer architecture is in little endian order, the <paramref name="value" /> will be
        /// reversed, because the <paramref name="value" /> is in network byte order (big endian order).
        /// </remarks>
        protected OpaqueOptionFormat(byte[] value)
            : base(value)
        {
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(OpaqueOptionFormat left, OpaqueOptionFormat right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(OpaqueOptionFormat left, OpaqueOptionFormat right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Indicates whether the current <see cref="OpaqueOptionFormat"/> is equal to another <see cref="OpaqueOptionFormat"/>.
        /// </summary>
        /// <param name="other">An <see cref="OpaqueOptionFormat"/> to compare with this <see cref="OpaqueOptionFormat"/>.</param>
        /// <returns>
        /// true if the current <see cref="OpaqueOptionFormat"/> is equal to the <paramref name="other">other</paramref> <see cref="OpaqueOptionFormat"/>; otherwise, false.
        /// </returns>
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

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Number, this.RawValue);
        }

        /// <summary>
        /// Converts the value of the <see cref="OpaqueOptionFormat" /> to its binary representation, for example, "48 65 6c 6c 6f".
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents the value of the <see cref="OpaqueOptionFormat" /> as its binary representation.
        /// </returns>
        public override string ToString() => BitConverter.ToString(this.RawValue).Replace('-', ' ');
    }
}
