// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Codes.Common
{
    using WorldDirect.CoAP.V1.Options;

    public class UnrecognizedOptionFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new UnrecognizedOption(src.Number, src.Value);
        }

        public int Number => -1;
    }
}
