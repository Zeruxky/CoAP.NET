﻿// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Messages.Codes
{
    public sealed class EmptyCode : CoapCode
    {
        public EmptyCode()
            : base(new CodeClass((UInt3)0), new CodeDetail((UInt5)0))
        {
        }
    }
}
