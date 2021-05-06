// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    /// <summary>
    /// Represents a If-Match option defined by RFC 7252.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.OpaqueOption" />
    public class IfMatch : OpaqueOption
    {
        public const ushort NUMBER = 1;
        private const ushort MAX_LENGTH = 8;
        private const ushort MIN_LENGTH = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfMatch"/> class.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}"/> that contains the content of the <see cref="IfMatch"/> option.</param>
        public IfMatch(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, true)
        {
        }

        /// <summary>
        /// Provides functionality to create a <see cref="IfMatch"/> option from a <see cref="OptionData"/>.
        /// </summary>
        /// <seealso cref="WorldDirect.CoAP.IOptionFactory" />
        public class Factory : IOptionFactory
        {
            /// <inheritdoc />
            public int Number => NUMBER;

            /// <inheritdoc />
            /// <exception cref="ArgumentOutOfRangeException">The number of the <see cref="OptionData"/> does not match with the <see cref="IfMatch"/> number.</exception>
            public CoapOption Create(OptionData src)
            {
                if (src.Number != NUMBER)
                {
                    throw new ArgumentOutOfRangeException(nameof(src), src.Length, "Option data number is not valid for If-Match factory.");
                }

                return new IfMatch(src.Value);
            }
        }
    }
}
