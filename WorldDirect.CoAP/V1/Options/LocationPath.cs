namespace WorldDirect.CoAP.V1.Options
{
    using System;

    /// <summary>
    /// Represents the Location-Path option defined by RFC 7252.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.StringOption" />
    public class LocationPath : StringOption
    {
        private const ushort NUMBER = 8;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationPath"/> class.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value of the <see cref="LocationPath"/>.</param>
        public LocationPath(string value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationPath"/> class.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}"/> that contains the content of the <see cref="LocationPath"/>.</param>
        public LocationPath(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, true)
        {
        }

        /// <summary>
        /// Provides functionality to create a <see cref="LocationPath"/> option from a <see cref="OptionData"/>.
        /// </summary>
        /// <seealso cref="WorldDirect.CoAP.IOptionFactory" />
        public class Factory : IOptionFactory
        {
            /// <inheritdoc />
            public int Number => LocationPath.NUMBER;

            /// <inheritdoc />
            public CoapOption Create(OptionData src)
            {
                return new LocationPath(src.Value);
            }
        }
    }
}
