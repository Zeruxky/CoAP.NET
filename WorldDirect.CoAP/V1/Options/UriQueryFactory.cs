namespace WorldDirect.CoAP.V1.Options
{
    public class UriQueryFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new UriQuery(src.Value);
        }

        public int Number => UriQuery.NUMBER;
    }
}
