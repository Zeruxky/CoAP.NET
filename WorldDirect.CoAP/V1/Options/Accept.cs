// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class Accept : UIntOptionFormat
    {
        public const ushort NUMBER = 17;
        private const ushort MIN_LENGTH = 0;
        private const ushort MAX_LENGTH = 2;

        public Accept(uint value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH)
        {
        }

        public Accept(byte[] value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH)
        {

        }
    }
}
