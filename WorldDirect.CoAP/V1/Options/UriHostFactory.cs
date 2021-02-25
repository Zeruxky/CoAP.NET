// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1
{
    using System;
    using System.Text;
    using WorldDirect.CoAP.V1.Options;

    public class UriHostFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new UriHost(src.StringValue);
        }

        public int Number => UriHost.NUMBER;
    }
}
