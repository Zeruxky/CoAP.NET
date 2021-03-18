// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class IfMatchFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            if (src.Number != this.Number)
            {
                throw new ArgumentException($"Option data number {src.Number} is not valid for If-Match factory.");
            }

            return new IfMatch(src.Value);
        }

        public int Number => IfMatch.NUMBER;
    }
}
