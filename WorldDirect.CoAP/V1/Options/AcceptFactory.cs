namespace WorldDirect.CoAP.V1.Options
{
    public class AcceptFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new Accept(src.Value);
        }

        public int Number => Accept.NUMBER;
    }
}
