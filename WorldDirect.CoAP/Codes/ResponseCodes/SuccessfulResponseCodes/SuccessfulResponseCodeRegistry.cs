// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Codes.ResponseCodes.SuccessfulResponseCodes
{
    using System.Collections.Generic;
    using System.Linq;
    using CoAP.Common;

    public class SuccessfulResponseCodeRegistry : Registry<SuccessfulResponseCode>
    {
        protected SuccessfulResponseCodeRegistry(IEnumerable<SuccessfulResponseCode> codes)
            : base(codes)
        {
        }
    }
}
