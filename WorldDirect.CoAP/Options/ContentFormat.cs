namespace WorldDirect.CoAP.Options
{
    using System;

    public class ContentFormat : UIntOptionFormat
    {
        public ContentFormat(uint value)
            : base(value)
        {
            if (value > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Value for Content-Format can only be in range of {ushort.MinValue} - {ushort.MaxValue}.");
            }
        }

        public override ushort Number => 12;

        public override string ToString()
        {
            return $"Content-Format ({this.Number}): {this.Value}";
        }
    }
}