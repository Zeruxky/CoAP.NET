namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriPath : StringOptionFormat
    {
        public UriPath(string value)
            : base(value)
        {
            if (value.Length > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value for Uri-Path can only be in range of 0 - 255 characters.");
            }
        }

        public override ushort Number => 11;
    }
}