namespace WorldDirect.CoAP.V1.Options
{
    public class IfNoneMatch : EmptyOptionFormat
    {
        public const ushort NUMBER = 5;

        public IfNoneMatch()
            : base(NUMBER)
        {
        }
    }

    public class IfNoneMatchFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new IfNoneMatch();
        }

        public int Number => IfNoneMatch.NUMBER;
    }
}
