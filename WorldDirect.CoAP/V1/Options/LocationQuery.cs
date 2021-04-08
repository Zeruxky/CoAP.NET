namespace WorldDirect.CoAP.V1.Options
{
    using System;

    /// <summary>
    /// Represents the Location-Query option defined by RFC 7252.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.StringOption" />
    public class LocationQuery : StringOption
    {
        private const ushort NUMBER = 20;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationQuery"/> class.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value for the <see cref="LocationQuery"/>.</param>
        public LocationQuery(string value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationQuery"/> class.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}"/> that contains the content of the <see cref="LocationQuery"/>.</param>
        public LocationQuery(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, true)
        {
        }

        /// <summary>
        /// Provides functionality to create a <see cref="LocationQuery"/> option from a <see cref="OptionData"/>.
        /// </summary>
        /// <seealso cref="WorldDirect.CoAP.IOptionFactory" />
        public class Factory : IOptionFactory
        {
            /// <inheritdoc />
            public int Number => LocationQuery.NUMBER;

            /// <inheritdoc />
            public CoapOption Create(OptionData src)
            {
                return new LocationQuery(src.Value);
            }
        }
    }
}
