namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class ProxyUri : StringOptionFormat
    {
        public ProxyUri(string value)
            : base(value)
        {
            if (value.Length < 1 || value.Length > 1034)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value for Proxy-Uri can only be in range of 1 - 1034 characters.");
            }
        }

        public override ushort Number => 35;

        public override string ToString()
        {
            return $"Proxy-Uri ({this.Number}): {this.Value}";
        }
    }
}