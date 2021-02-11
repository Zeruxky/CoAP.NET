// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1
{
    using WorldDirect.CoAP.V1.Options;

    public class IfMatchFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new IfMatch(src.Value.ToArray());
        }

        public int Number => 1;
    }
}
