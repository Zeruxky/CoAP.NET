namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class LocationQuery : StringOptionFormat
    {
        public const ushort NUMBER = 20;

        public LocationQuery(string value)
            : base(NUMBER, value, 0, 255)
        {
        }
    }

    public class LocationQueryFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new LocationQuery(src.StringValue);
        }

        public int Number => LocationQuery.NUMBER;
    }
}
