namespace WorldDirect.CoAP.V1.Options
{
    using System;

    /// <summary>
    /// Represents the Size1 option defined in RFC 7252. It provides information about the resource
    /// representation in a request. Its main use is with block-wise transfers.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.UIntOption" />
    public class Size1 : UIntOption
    {
        private const ushort NUMBER = 60;
        private const ushort MAX_LENGTH = 4;
        private const ushort MIN_LENGTH = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Size1"/> class.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value for that <see cref="Size1"/> option.</param>
        public Size1(uint value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size1"/> class.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}"/> that contains the value for that <see cref="Size1"/> option.</param>
        public Size1(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        /// <summary>
        /// Provides functionality to create a <see cref="Size1"/> option from a specified <see cref="OptionData"/>.
        /// </summary>
        /// <seealso cref="WorldDirect.CoAP.IOptionFactory" />
        public class Factory : IOptionFactory
        {
            /// <inheritdoc />
            public int Number => Size1.NUMBER;

            /// <inheritdoc />
            public CoapOption Create(OptionData src)
            {
                return new Size1(src.Value);
            }
        }
    }
}
