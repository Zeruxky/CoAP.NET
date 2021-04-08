// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Codes
{
    using System.Collections.Generic;
    using WorldDirect.CoAP.Common;

    public class ResponseCodeRegistry : Registry<ResponseCode>
    {
        public ResponseCodeRegistry(IEnumerable<ResponseCode> codes)
            : base(codes)
        {
        }
    }
}
