namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class LocationQuery : StringOptionFormat
    {
        public LocationQuery(string value)
            : base(value)
        {
            if (value.Length > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value for Location-Query can only be in range of 0 - 255 characters.");
            }
        }

        public override ushort Number => 20;

        public override string ToString()
        {
            return $"Location-Query ({this.Number}): {this.Value}";
        }
    }
}