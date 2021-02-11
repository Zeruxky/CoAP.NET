// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;

    public ref struct OptionData
    {
        public OptionData(ushort number, ushort length, ReadOnlySpan<byte> value)
        {
            this.Number = number;
            this.Length = length;
            this.Value = value;
        }

        public ushort Number { get; }

        public ushort Length { get; }

        public ReadOnlySpan<byte> Value { get; }
    }
}