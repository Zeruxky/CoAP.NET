namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class ProxyUri : StringOptionFormat
    {
        public const ushort NUMBER = 35;
        private const ushort MAX_LENGTH = 1034;
        private const ushort MIN_LENGTH = 1;

        public ProxyUri(string value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }

        public ProxyUri(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }
    }
}
