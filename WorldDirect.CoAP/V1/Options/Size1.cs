namespace WorldDirect.CoAP.V1.Options
{
    public class Size1 : UIntOptionFormat
    {
        public const ushort NUMBER = 60;
        private const ushort MAX_LENGTH = 4;
        private const ushort MIN_LENGTH = 0;

        public Size1(uint value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }

        public Size1(byte[] value)
            : base(NUMBER, value, MAX_LENGTH, MIN_LENGTH)
        {
        }
    }
}
