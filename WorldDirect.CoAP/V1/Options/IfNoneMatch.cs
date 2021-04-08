namespace WorldDirect.CoAP.V1.Options
{
    using System;

    /// <summary>
    /// Represents the If-None-Match option defined by RFC 7252.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.EmptyOption" />
    public class IfNoneMatch : EmptyOption
    {
        private const ushort NUMBER = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfNoneMatch"/> class.
        /// </summary>
        public IfNoneMatch()
            : base(NUMBER)
        {
        }

        /// <summary>
        /// Provides functionality to create a <see cref="IfNoneMatch"/> option from a <see cref="OptionData"/>.
        /// </summary>
        /// <seealso cref="WorldDirect.CoAP.IOptionFactory" />
        public class Factory : IOptionFactory
        {
            /// <inheritdoc />
            public int Number => NUMBER;

            /// <inheritdoc />
            /// <exception cref="ArgumentOutOfRangeException">The number of the <see cref="OptionData"/> does not match with the <see cref="IfNoneMatch"/> number.</exception>
            public CoapOption Create(OptionData src)
            {
                if (src.Number != NUMBER)
                {
                    throw new ArgumentOutOfRangeException(nameof(src), src.Length, "Option data number is not valid for If-None factory.");
                }

                return new IfNoneMatch();
            }
        }
    }
}
