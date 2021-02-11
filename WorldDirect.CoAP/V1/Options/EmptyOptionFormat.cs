// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    /// <summary>
    /// Represents a <see cref="CoapOption"/> as empty. This means the value of this <see cref="CoapOption"/>
    /// is a zero-length sequence of bytes.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.Messages.Options.CoapOption" />
    /// <seealso cref="System.IEquatable{WorldDirect.CoAP.Messages.Options.EmptyOptionFormat}" />
    public abstract class EmptyOptionFormat : CoapOption, IEquatable<EmptyOptionFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyOptionFormat"/> class.
        /// </summary>
        protected EmptyOptionFormat()
            : base(new byte[0])
        {
        }

        /// <summary>
        /// Gets the raw value of that <see cref="CoapOption" /> in the system's computer architecture.
        /// </summary>
        /// <value>
        /// The raw value.
        /// </value>
        public byte[] RawValue { get; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(EmptyOptionFormat left, EmptyOptionFormat right)
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
        public static bool operator !=(EmptyOptionFormat left, EmptyOptionFormat right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Indicates whether the current <see cref="EmptyOptionFormat" /> is equal to another <see cref="EmptyOptionFormat" />.
        /// </summary>
        /// <param name="other">An <see cref="EmptyOptionFormat" /> to compare with this <see cref="EmptyOptionFormat" />.</param>
        /// <returns>
        /// true if the current <see cref="EmptyOptionFormat" /> is equal to the <paramref name="other">other</paramref> <see cref="EmptyOptionFormat" />; otherwise, false.
        /// </returns>
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

            return this.Equals((EmptyOptionFormat) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Number.GetHashCode();
        }
    }
}
