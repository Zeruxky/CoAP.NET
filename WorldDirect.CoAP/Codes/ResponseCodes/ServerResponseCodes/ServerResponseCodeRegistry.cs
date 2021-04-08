// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Codes.ResponseCodes.ServerResponseCodes
{
    using System.Collections.Generic;
    using System.Linq;
    using CoAP.Common;

    public class ServerResponseCodeRegistry : Registry<ServerResponseCode>
    {
        public ServerResponseCodeRegistry(IEnumerable<ServerResponseCode> codes)
            : base(codes)
        {
        }
    }
}
