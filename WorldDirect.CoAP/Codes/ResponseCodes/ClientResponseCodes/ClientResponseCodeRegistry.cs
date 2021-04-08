// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Codes.ResponseCodes.ClientResponseCodes
{
    using System.Collections.Generic;
    using System.Linq;
    using WorldDirect.CoAP.Common;

    public class ClientResponseCodeRegistry : Registry<ClientResponseCode>
    {
        public ClientResponseCodeRegistry(IEnumerable<ClientResponseCode> codes)
            : base(codes)
        {
        }
    }
}
