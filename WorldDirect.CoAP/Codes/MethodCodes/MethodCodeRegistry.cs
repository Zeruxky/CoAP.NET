// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Codes.MethodCodes
{
    using System.Collections.Generic;
    using System.Linq;
    using WorldDirect.CoAP.Common;

    public class MethodCodeRegistry : Registry<MethodCode>
    {
        protected MethodCodeRegistry(IEnumerable<MethodCode> codes)
            : base(codes)
        {
        }
    }
}
