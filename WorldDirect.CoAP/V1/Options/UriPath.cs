namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriPath : StringOption
    {
        public const ushort NUMBER = 11;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 0;

        public UriPath(string value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, true)
        {
        }

        public UriPath(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, true)
        {
        }

        public class Factory : IOptionFactory
        {
            /// <inheritdoc />
            public int Number => NUMBER;

            /// <inheritdoc />
            public CoapOption Create(OptionData src)
            {
                if (src.Number != NUMBER)
                {
                    throw new ArgumentException($"Option data number {src.Number} is not valid for Uri-Path factory.");
                }

                return new UriPath(src.Value);
            }
        }
    }
}
