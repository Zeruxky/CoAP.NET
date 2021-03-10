// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriHost : StringOptionFormat
    {
        public const ushort NUMBER = 3;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 1;

        public UriHost(string value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }

        public UriHost(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }
    }
}
