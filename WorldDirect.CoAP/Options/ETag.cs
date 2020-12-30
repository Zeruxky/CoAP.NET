namespace WorldDirect.CoAP.Options
{
    using System;

    public class ETag : OpaqueOptionFormat
    {
        public ETag(byte[] value)
            : base(value)
        {
            if (value.Length < 1 || value.Length > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value for ETag option can only be in range of 1 - 8 bytes.");
            }
        }

        public override ushort Number => 3;

        public override string ToString()
        {
            return $"ETag ({this.Number}): {this.RawValue}";
        }
    }
}