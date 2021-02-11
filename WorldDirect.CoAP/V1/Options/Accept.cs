// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class Accept : UIntOptionFormat
    {
        public Accept(uint value)
            : base(value)
        {
            if (value > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Value for Accept can only be in range of {ushort.MinValue} - {ushort.MaxValue}.");
            }
        }

        public override ushort Number => 17;

        public override string ToString()
        {
            return $"Accept ({this.Number}): {this.Value}";
        }
    }
}
