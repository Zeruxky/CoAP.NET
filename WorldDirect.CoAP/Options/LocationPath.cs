namespace WorldDirect.CoAP.Options
{
    using System;
    using System.Text;

    public class LocationPath : StringOptionFormat
    {
        public LocationPath(string value)
            : base(value, Encoding.UTF8)
        {
            if (value.Length > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value for Location-Path can only be in range of 0 - 255 characters.");
            }
        }

        public override ushort Number => 8;

        public override string ToString()
        {
            return $"Location-Path ({this.Number}): {this.Value}";
        }
    }
}