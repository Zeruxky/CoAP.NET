// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a <see cref="CoapOption"/> in <see cref="string"/> format. This means the value of that <see cref="CoapOption"/>
    /// represents a unicode <see cref="string"/> that is encoded using UTF-8 in Net-Unicode form.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.Messages.Options.CoapOption" />
    /// <seealso cref="System.IEquatable{WorldDirect.CoAP.Messages.Options.StringOptionFormat}" />
    public abstract class StringOptionFormat : CoapOption, IEquatable<StringOptionFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringOptionFormat"/> class.
        /// </summary>
        /// <param name="value">The byte array (in network byte order) that represents the value of that <see cref="CoapOption"/>.</param>
        /// <remarks>
        /// To be compliant with the RFC 7252, we encoding the byte array by using UTF-8 and
        /// normalize the string by using the Unicode normalization form "NFC".
        /// </remarks>
        protected StringOptionFormat(byte[] value)
            : base(value)
        {
            this.Value = new UTF8Encoding(false)
                .GetString(this.RawValue)
                .Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringOptionFormat"/> class.
        /// </summary>
        /// <param name="value">The <see cref="string"/> that represents the value of that <see cref="CoapOption"/>.</param>
        protected StringOptionFormat(string value)
            : this(new UTF8Encoding(false).GetBytes(value.Normalize(NormalizationForm.FormC)))
        {
        }

        /// <summary>
        /// Gets the value of this <see cref="CoapOption"/> as <see cref="string"/>.
        /// </summary>
        /// <value>
        /// The value of this <see cref="CoapOption"/> as <see cref="string"/>.
        /// </value>
        /// <remarks>
        /// The byte array of that <see cref="CoapOption"/> will be encoded by using UTF-8 encoding.
        /// Also the string is in Net-Unicode form (specified by RFC 5198). This means, the encoded
        /// UTF-8 string must also be normalized by using the Unicode normalization form "NFC".
        /// </remarks>
        public string Value { get; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(StringOptionFormat left, StringOptionFormat right)
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
        public static bool operator !=(StringOptionFormat left, StringOptionFormat right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Indicates whether the current <see cref="StringOptionFormat" /> is equal to another <see cref="StringOptionFormat" />.
        /// </summary>
        /// <param name="other">An <see cref="StringOptionFormat" /> to compare with this <see cref="StringOptionFormat" />.</param>
        /// <returns>
        /// true if the current <see cref="StringOptionFormat" /> is equal to the <paramref name="other">other</paramref> <see cref="StringOptionFormat" />; otherwise, false.
        /// </returns>
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

            return Equals(this.Number, other.Number) && this.RawValue.SequenceEqual(other.RawValue) && this.Value.Equals(other.Value);
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

            return this.Equals((StringOptionFormat) obj);
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

        public override string ToString() => this.Value;
    }
}
