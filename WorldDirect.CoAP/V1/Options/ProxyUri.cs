namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class ProxyUri : StringOption
    {
        public const ushort NUMBER = 35;
        private const ushort MAX_LENGTH = 1034;
        private const ushort MIN_LENGTH = 1;

        public ProxyUri(string value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        public ProxyUri(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        public class Factory : IOptionFactory
        {
            public int Number => ProxyUri.NUMBER;

            public CoapOption Create(OptionData src)
            {
                return new ProxyUri(src.Value);
            }
        }
    }
}
