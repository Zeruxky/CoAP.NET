﻿// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    public struct OptionsLength
    {
        public OptionsLength(ushort value, byte size)
        {
            this.Value = value;
            this.Size = size;
        }

        public ushort Value { get; }

        public byte Size { get; }
    }
}