// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriHostFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            if (src.Number != this.Number)
            {
                throw new ArgumentException($"Option data number {src.Number} is not valid for Uri-Host factory.");
            }

            return new UriHost(src.Value);
        }

        public int Number => UriHost.NUMBER;
    }
}
