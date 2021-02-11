// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Codes
{
    using WorldDirect.CoAP.Common;

    public class RequestCode : CoapCode
    {
        public RequestCode(CodeDetail detail)
            : base(new CodeClass((UInt3)0), detail)
        {
        }
    }
}
