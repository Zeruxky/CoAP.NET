namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class MaxAge : UIntOption
    {
        public const ushort NUMBER = 14;
        private const ushort MAX_LENGTH = 4;
        private const ushort MIN_LENGTH = 0;

        public MaxAge(uint value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        public MaxAge(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        public class Factory : IOptionFactory
        {
            public int Number => MaxAge.NUMBER;

            public CoapOption Create(OptionData src)
            {
                return new MaxAge(src.Value);
            }
        }
    }
}
