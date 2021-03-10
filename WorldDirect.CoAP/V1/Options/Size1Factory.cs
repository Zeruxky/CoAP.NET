﻿namespace WorldDirect.CoAP.V1.Options
{
    public class Size1Factory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new Size1(src.Value);
        }

        public int Number => Size1.NUMBER;
    }
}
