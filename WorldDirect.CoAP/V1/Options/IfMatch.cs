// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class IfMatch : OpaqueOptionFormat
    {
        public const ushort NUMBER = 1;
        private const ushort MAX_LENGTH = 8;
        private const ushort MIN_LENGTH = 0;

        public IfMatch(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }
    }
}
