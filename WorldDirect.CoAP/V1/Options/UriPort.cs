// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriPort : UIntOptionFormat
    {
        public const ushort NUMBER = 7;
        private const ushort MAX_LENGTH = 2;
        private const ushort MIN_LENGTH = 0;

        public UriPort(uint value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }

        public UriPort(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }
    }
}
