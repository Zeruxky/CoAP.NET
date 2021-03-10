namespace WorldDirect.CoAP.V1.Options
{
    public class UriPortFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new UriPort(src.Value);
        }

        public int Number => UriPort.NUMBER;
    }
}
