namespace WorldDirect.CoAP.Messages.Options
{
    using System;

    public class UriHost : StringOptionFormat
    {
        public UriHost(string value)
            : base(value)
        {
            if (value.Length < 1 || value.Length > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value for Uri-Host can only be in range of 1 - 255 characters.");
            }
        }

        public override ushort Number => 3;

        public override string ToString()
        {
            return $"Uri-Host ({this.Number}): {this.Value}";
        }
    }
}
