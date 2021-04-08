// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    /// <summary>
    /// Represents the Accept option defined at RFC 7252. It indicates which <see cref="ContentFormat"/>
    /// is acceptable for the client.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.UIntOption" />
    public class Accept : UIntOption
    {
        private const ushort NUMBER = 17;
        private const ushort MAX_LENGTH = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Accept"/> class.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value for that <see cref="Accept"/> option.</param>
        public Accept(uint value)
            : base(NUMBER, value, MAX_LENGTH, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Accept"/> class.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}"/> value that contains the value for that <see cref="Accept"/> option.</param>
        public Accept(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MAX_LENGTH, false)
        {
        }

        /// <summary>
        /// Provides functionality to create a <see cref="Accept"/> options from a specified <see cref="OptionData"/>.
        /// </summary>
        /// <seealso cref="WorldDirect.CoAP.IOptionFactory" />
        public class Factory : IOptionFactory
        {
            /// <inheritdoc />
            public int Number => NUMBER;

            /// <inheritdoc />
            /// <exception cref="ArgumentOutOfRangeException">Throws if the number of the specified <see cref="OptionData"/> is not equal to the <see cref="Factory"/>s number.</exception>
            public CoapOption Create(OptionData src)
            {
                if (src.Number != NUMBER)
                {
                    throw new ArgumentOutOfRangeException(nameof(src), src.Number, "Option data number is not valid for Accept factory.");
                }

                return new Accept(src.Value);
            }
        }
    }
}
