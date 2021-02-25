namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class ProxyScheme : StringOptionFormat
    {
        public const ushort NUMBER = 39;

        public ProxyScheme(string value)
            : base(NUMBER, value, 1, 255)
        {
        }
    }

    public class ProxySchemeFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new ProxyScheme(src.StringValue);
        }

        public int Number => ProxyScheme.NUMBER;
    }
}
