namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class ProxyScheme : StringOption
    {
        public const ushort NUMBER = 39;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 1;

        public ProxyScheme(string value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        public ProxyScheme(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        public class Factory : IOptionFactory
        {
            public int Number => ProxyScheme.NUMBER;

            public CoapOption Create(OptionData src)
            {
                return new ProxyScheme(src.Value);
            }
        }
    }
}
