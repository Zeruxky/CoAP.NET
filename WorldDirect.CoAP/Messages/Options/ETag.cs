namespace WorldDirect.CoAP.Messages.Options
{
    using System;
    using System.Text;

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

        public override ushort Number => 4;

        public override string ToString()
        {
            return $"ETag ({this.Number}): {Encoding.UTF8.GetString(this.RawValue)}";
        }
    }
}
