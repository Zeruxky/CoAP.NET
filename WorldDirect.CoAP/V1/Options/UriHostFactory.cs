// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1
{
    using System.Text;
    using WorldDirect.CoAP.V1.Options;

    public class UriHostFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            var stringValue = Encoding.UTF8.GetString(src.Value.ToArray());
            return new UriHost(stringValue);
        }

        public int Number => 3;
    }
}
