namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class LocationQuery : StringOptionFormat
    {
        public const ushort NUMBER = 20;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 0;

        public LocationQuery(string value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }

        public LocationQuery(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }
    }
}
