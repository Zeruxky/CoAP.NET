// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    /// <summary>
    /// Represents the ETag option specified by RFC 7252.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.OpaqueOption" />
    public class ETag : OpaqueOption
    {
        private const ushort NUMBER = 4;
        private const ushort MAX_LENGTH = 8;
        private const ushort MIN_LENGTH = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ETag"/> class.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}"/> that holds the value of the <see cref="ETag"/> option.</param>
        public ETag(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, true)
        {
        }

        /// <summary>
        /// Provides functionality to create a <see cref="ETag"/> option from the specified <see cref="OptionData"/>.
        /// </summary>
        /// <seealso cref="WorldDirect.CoAP.IOptionFactory" />
        public class Factory : IOptionFactory
        {
            /// <inheritdoc />
            public int Number => NUMBER;

            /// <inheritdoc />
            /// <exception cref="ArgumentOutOfRangeException">src - Option data number is not valid for ETag factory.</exception>
            public CoapOption Create(OptionData src)
            {
                if (src.Number != NUMBER)
                {
                    throw new ArgumentOutOfRangeException(nameof(src), src.Length, "Option data number is not valid for ETag factory.");
                }

                return new ETag(src.Value);
            }
        }
    }
}
