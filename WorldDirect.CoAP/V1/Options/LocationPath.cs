namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Collections.Specialized;

    public class LocationPath : StringOptionFormat
    {
        public const ushort NUMBER = 8;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 0;

        public LocationPath(string value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }

        public LocationPath(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {

        }
    }
}
