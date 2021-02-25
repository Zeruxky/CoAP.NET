namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriQuery : StringOptionFormat
    {
        public const ushort NUMBER = 15;

        public UriQuery(string value)
            : base(NUMBER, value, 0, 255)
        {
        }
    }

    public class UriQueryFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new UriQuery(src.StringValue);
        }

        public int Number => UriQuery.NUMBER;
    }
}
