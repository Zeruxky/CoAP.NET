﻿namespace WorldDirect.CoAP.V1.Options
{
    public class LocationPathFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new LocationPath(src.Value);
        }

        public int Number => LocationPath.NUMBER;
    }
}
