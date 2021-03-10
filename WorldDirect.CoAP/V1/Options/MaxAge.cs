namespace WorldDirect.CoAP.V1.Options
{
    public class MaxAge : UIntOptionFormat
    {
        public const ushort NUMBER = 14;
        private const ushort MAX_LENGTH = 4;
        private const ushort MIN_LENGTH = 0;

        public MaxAge(uint value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }

        public MaxAge(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }
    }
}
