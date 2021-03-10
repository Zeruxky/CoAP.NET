namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class ProxyScheme : StringOptionFormat
    {
        public const ushort NUMBER = 39;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 1;

        public ProxyScheme(string value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }

        public ProxyScheme(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }
    }
}
