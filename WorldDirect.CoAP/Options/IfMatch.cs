﻿namespace WorldDirect.CoAP.Options
{
    using System;

    public class IfMatch : OpaqueOptionFormat
    {
        public IfMatch(byte[] value)
            : base(value)
        {
            if (value.Length > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value for If-Match option can only be in range of 0 - 8 bytes.");
            }
        }

        public override ushort Number => 1;

        public override string ToString()
        {
            return $"If-Match ({this.Number}): {this.RawValue}";
        }
    }
}
