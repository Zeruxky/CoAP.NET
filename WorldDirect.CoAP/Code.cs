// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System.Net;

    /// <summary>
    /// Represents a 8-bit unsigned integer and appears after the <see cref="Type"/> of the CoAP message.
    /// It is split into a 3-bit class (most significant bits) and a 5-bit detail (least significant bits).
    /// </summary>
    /// <remarks>
    /// The format of a <see cref="Code"/> is as follows: <c>c.dd</c> where <c>c</c> is a digit from 0 to 7
    /// for the 3-bit subfield and <c>dd</c> are two digits from 00 to 31 for the 5-bit subfield.
    ///
    /// Possible values are maintained at Section 12.1 in RFC 7252.
    /// </remarks>
    public class Code
    {
        private const byte CLASS_MASK = 0xE0;
        private const byte DETAIL_MASK = 0x07;
        private readonly byte value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Code"/> class.
        /// </summary>
        /// <param name="class">The <see cref="CodeClass"/> for this <see cref="Code"/>.</param>
        /// <param name="detail">The <see cref="CodeDetail"/> for this <see cref="Code"/>.</param>
        public Code(CodeClass @class, CodeDetail detail)
        {
            this.value = (byte)(((UInt3)@class << 5) | (UInt5)detail);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Code"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Code(byte value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the subfield of the <see cref="Code"/> that represents the class.
        /// </summary>
        /// <value>
        /// The class.
        /// </value>
        public CodeClass Class => (CodeClass)(UInt3)((this.value & CLASS_MASK) >> 5);

        /// <summary>
        /// Gets the subfield of the <see cref="Code"/> that represents the detail.
        /// </summary>
        /// <value>
        /// The detail.
        /// </value>
        public CodeDetail Detail => (CodeDetail)(UInt5)(this.value & DETAIL_MASK);

        /// <summary>
        /// Performs an explicit conversion from <see cref="byte"/> to <see cref="Code"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The <see cref="Code"/> that is equivalent to the <see cref="byte"/> value.
        /// </returns>
        public static explicit operator Code(byte value) => new Code(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Code"/> to <see cref="byte"/>.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>
        /// The <see cref="byte"/> that is equivalent to the <paramref name="code"/>.
        /// </returns>
        public static implicit operator byte(Code code) => code.value;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Class}.{this.Detail}";
        }
    }
}
