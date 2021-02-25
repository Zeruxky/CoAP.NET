﻿namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Collections.Specialized;

    public class LocationPath : StringOptionFormat
    {
        public const ushort NUMBER = 8;

        public LocationPath(string value)
            : base(NUMBER, value, 0, 255)
        {
        }
    }

    public class LocationPathFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new LocationPath(src.StringValue);
        }

        public int Number => LocationPath.NUMBER;
    }
}
