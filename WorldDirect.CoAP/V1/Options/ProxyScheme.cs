namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class ProxyScheme : StringOptionFormat
    {
        public ProxyScheme(string value)
            : base(value)
        {
            if (value.Length < 1 || value.Length > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value for Proxy-Scheme can only be in range of 1 - 255 characters.");
            }
        }

        public override ushort Number => 39;

        public override string ToString()
        {
            return $"Proxy-Scheme ({this.Number}): {this.Value}";
        }
    }
}