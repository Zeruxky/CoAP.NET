// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class ETag : OpaqueOptionFormat
    {
        public const ushort NUMBER = 4;
        private const ushort MAX_LENGTH = 8;
        private const ushort MIN_LENGTH = 1;

        public ETag(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }
    }
}
