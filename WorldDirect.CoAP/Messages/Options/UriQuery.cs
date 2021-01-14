namespace WorldDirect.CoAP.Messages.Options
{
    using System;

    public class UriQuery : StringOptionFormat
    {
        public UriQuery(string value)
            : base(value)
        {
            if (value.Length > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value for Uri-Query can only be in range of 0 - 255 characters.");
            }
        }

        public override ushort Number => 15;

        public override string ToString()
        {
            return $"Uri-Query ({this.Number}): {this.Value}";
        }
    }
}