namespace WorldDirect.CoAP.V1.Options
{
    public class ProxyUriFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new ProxyUri(src.Value);
        }

        public int Number => ProxyUri.NUMBER;
    }
}
