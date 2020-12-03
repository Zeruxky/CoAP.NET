// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    /// <summary>
    /// Represents the token length field (TKL) in a CoAP message.
    /// It indicates the length of the variable-length Token field.
    /// </summary>
    /// <remarks>
    /// With RFC 7252 the length can only be between 0 and 8 bytes. Values
    /// between 9 - 15 are reserved and leading in a <see cref="MessageFormatError"/>.
    /// </remarks>
    public class TokenLength
    {
        private readonly UInt4 value;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenLength"/> class.
        /// </summary>
        /// <param name="value">The <see cref="UInt4"/> that indicates the length of the variable-length Token field.</param>
        /// <exception cref="MessageFormatError">If the <paramref name="value"/> is greater than 8.</exception>
        public TokenLength(UInt4 value)
        {
            if (value > 8)
            {
                throw new MessageFormatError($"The {nameof(TokenLength)} indicates a length of more than 8 bytes. According to RFC 7252 lengths 9 - 15 bytes are reserved.");
            }

            this.value = value;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="UInt4"/> to <see cref="TokenLength"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The <see cref="TokenLength"/> that is equivalent to the <paramref name="value"/>.
        /// </returns>
        public static explicit operator TokenLength(UInt4 value) => new TokenLength(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="TokenLength"/> to <see cref="UInt4"/>.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>
        /// The <see cref="UInt4"/> that is equivalent to the <paramref name="length"/>.
        /// </returns>
        public static implicit operator UInt4(TokenLength length) => length.value;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if both <see cref="TokenLength"/>s are equal, otherwise <c>false</c>.
        /// </returns>
        public static bool operator ==(TokenLength left, TokenLength right) => Equals(left, right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if both <see cref="TokenLength"/>s are not equal, otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(TokenLength left, TokenLength right) => !Equals(left, right);

        /// <summary>
        /// Indicates if the given <see cref="UInt2"/> is equal to the current <see cref="UInt2"/>.
        /// </summary>
        /// <param name="other">The other <see cref="UInt2"/>.</param>
        /// <returns><c>true</c> if the <paramref name="other"/> <see cref="UInt2"/> is equal to the current <see cref="UInt2"/>, otherwise <c>false</c>.</returns>
        public bool Equals(TokenLength other)
        {
            return this.value == other.value;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((TokenLength)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.value.ToString();
        }
    }
}
