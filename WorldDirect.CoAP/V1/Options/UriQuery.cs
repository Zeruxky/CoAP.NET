namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriQuery : StringOption
    {
        public const ushort NUMBER = 15;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 0;

        public UriQuery(string value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, true)
        {
        }

        public UriQuery(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, true)
        {
        }

        public class Factory : IOptionFactory
        {
            public int Number => NUMBER;

            public CoapOption Create(OptionData src)
            {
                return new UriQuery(src.Value);
            }
        }
    }
}
