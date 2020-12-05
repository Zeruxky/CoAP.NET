// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    /// <summary>
    /// Represents a two bit unsigned integer of a CoAP message and appears after the <see cref="Version"/>. It indicates
    /// if this message is of type Confirmable (0), Non-Confirmable(1), Acknowledgement (2), or Reset (3).
    /// </summary>
    /// <remarks>
    /// The semantics of these message <see cref="Type"/>s are defined in Section 4 of RFC 7252.
    /// </remarks>
    public class Type
    {
        /// <summary>
        /// The <see cref="Type"/> that represents a CoAP message as Confirmable (0).
        /// </summary>
        /// <remarks>
        /// The value is specified by RFC 7252.
        /// </remarks>
        public static readonly Type Confirmable = new Type((UInt2)0);

        /// <summary>
        /// The <see cref="Type"/> that represents a CoAP message as Non-Confirmable (1).
        /// </summary>
        /// <remarks>
        /// The value is specified by RFC 7252.
        /// </remarks>
        public static readonly Type NonConfirmable = new Type((UInt2)1);

        /// <summary>
        /// The <see cref="Type"/> that represents a CoAP message as Acknowledgment (2).
        /// </summary>
        /// <remarks>
        /// The value is specified by RFC 7252.
        /// </remarks>
        public static readonly Type Acknowledgement = new Type((UInt2)2);

        /// <summary>
        /// The <see cref="Type"/> that represents a CoAP message as Reset (3).
        /// </summary>
        /// <remarks>
        /// The value is specified by RFC 7252.
        /// </remarks>
        public static readonly Type Reset = new Type((UInt2)3);

        private readonly UInt2 value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Type"/> class.
        /// </summary>
        /// <param name="value">The <see cref="UInt2"/> value that represents the type of a CoAP message.</param>
        public Type(UInt2 value)
        {
            this.value = value;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="UInt2"/> to <see cref="Type"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The <see cref="Type"/> that is equivalent to the <paramref name="value"/>.
        /// </returns>
        public static explicit operator Type(UInt2 value) => new Type(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Type"/> to <see cref="UInt2"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The <see cref="UInt2"/> that is equivalent to the <paramref name="type"/>.
        /// </returns>
        public static implicit operator UInt2(Type type) => type.value;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if both <see cref="Type"/>s are equal, otherwise <c>false</c>.
        /// </returns>
        public static bool operator ==(Type left, Type right) => Equals(left, right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if both <see cref="Type"/>s are not equal, otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(Type left, Type right) => !Equals(left, right);

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

            return this.Equals((Type)obj);
        }

        /// <summary>
        /// Indicates if the specified <see cref="Type"/> is equal to the current <see cref="Type"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Type"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Type"/> is equal to the current <see cref="Type"/>.</returns>
        public bool Equals(Type other)
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
