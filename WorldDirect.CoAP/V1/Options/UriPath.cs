namespace WorldDirect.CoAP.V1.Options
{
    public class UriPath : StringOptionFormat
    {
        public const ushort NUMBER = 11;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 0;

        public UriPath(string value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }

        public UriPath(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }
    }
}
