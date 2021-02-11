// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a <see cref="CoapOption"/> as in <see cref="uint"/> format. This means the value
    /// of the <see cref="CoapOption"/> is a <see cref="uint"/>.
    /// </summary>
    public abstract class UIntOptionFormat : CoapOption, IEquatable<UIntOptionFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIntOptionFormat"/> class.
        /// </summary>
        /// <param name="value">The byte array (in network byte order) that represents the value of that option.</param>
        /// <remarks>
        /// If the system's computer architecture is in little endian order, the <paramref name="value"/> will be
        /// reversed, because the <paramref name="value"/> is in network byte order (big endian order).
        /// </remarks>
        protected UIntOptionFormat(byte[] value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIntOptionFormat"/> class.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value of that <see cref="CoapOption"/>.</param>
        /// <remarks>
        /// Depending on the system's computer architecture, the specified <paramref name="value"/> will
        /// be reversed. If the system's computer architecture is in little endian order, the <paramref name="value"/>
        /// will be reversed; otherwise it will not.
        /// </remarks>
        protected UIntOptionFormat(uint value)
            : base(UIntOptionFormat.ToNetworkByteOrder(value))
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public uint Value => BitConverter.ToUInt32(this.RawValue, 0);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(UIntOptionFormat left, UIntOptionFormat right)
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
        public static bool operator !=(UIntOptionFormat left, UIntOptionFormat right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Indicates whether the current <see cref="UIntOptionFormat"/> is equal to another <see cref="UIntOptionFormat"/>.
        /// </summary>
        /// <param name="other">An <see cref="UIntOptionFormat"/> to compare with this <see cref="UIntOptionFormat"/>.</param>
        /// <returns>
        /// true if the current <see cref="UIntOptionFormat"/> is equal to the <paramref name="other">other</paramref> <see cref="UIntOptionFormat"/>; otherwise, false.
        /// </returns>
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

            return this.Number == other.Number && this.Value.Equals(other.Value) && this.RawValue.SequenceEqual(other.RawValue);
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

            return this.Equals((UIntOptionFormat)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Number, this.RawValue, this.Value);
        }

        /// <inheritdoc />
        public override string ToString() => this.Value.ToString("D");

        private static byte[] ToNetworkByteOrder(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }
    }
}
