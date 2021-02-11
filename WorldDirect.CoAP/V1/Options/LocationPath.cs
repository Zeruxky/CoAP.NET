namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class LocationPath : StringOptionFormat
    {
        public LocationPath(string value)
            : base(value)
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
