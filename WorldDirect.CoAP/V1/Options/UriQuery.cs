namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriQuery : StringOptionFormat
    {
        public const ushort NUMBER = 15;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 0;

        public UriQuery(string value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }

        public UriQuery(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }
    }
}
