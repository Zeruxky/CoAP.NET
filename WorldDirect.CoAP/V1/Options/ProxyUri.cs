namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class ProxyUri : StringOptionFormat
    {
        public const ushort NUMBER = 35;

        public ProxyUri(string value)
            : base(NUMBER, value, 1, 1034)
        {
        }
    }

    public class ProxyUriFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new ProxyUri(src.StringValue);
        }

        public int Number => ProxyUri.NUMBER;
    }
}
