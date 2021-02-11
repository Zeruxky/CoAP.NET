// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriPort : UIntOptionFormat
    {
        public UriPort(uint value)
            : base(value)
        {
            if (value > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Value for Uri-Host option can only be in range of {ushort.MinValue} - {ushort.MaxValue}.");
            }
        }

        public override ushort Number => 7;

        public override string ToString()
        {
            return $"Uri-Port ({this.Number}): {this.Value}";
        }
    }
}
