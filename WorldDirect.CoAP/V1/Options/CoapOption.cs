namespace WorldDirect.CoAP.V1.Options
{
    using System;

    /// <summary>
    /// Represents a option specified by RFC 7252.
    /// </summary>
    public abstract class CoapOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoapOption"/> class.
        /// </summary>
        /// <param name="value">The byte array (in network byte order) that represents the value of that option.</param>
        /// <remarks>
        /// If the system's computer architecture is in little endian order, the <paramref name="value"/>
        /// will be reversed, because the <paramref name="value"/> is expected to be in network byte order (big endian order).
        /// </remarks>
        protected CoapOption(byte[] value)
        {
            // Check if the array must be reversed to be little endian.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(value);
            }

            this.RawValue = value;
        }

        /// <summary>
        /// Gets the number of that <see cref="CoapOption"/>.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public abstract ushort Number { get; }

        /// <summary>
        /// Gets the raw value of that <see cref="CoapOption"/> in the system's computer architecture.
        /// </summary>
        /// <value>
        /// The raw value.
        /// </value>
        public byte[] RawValue { get; }
    }
}