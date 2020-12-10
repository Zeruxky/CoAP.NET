namespace WorldDirect.CoAP
{
    using System;

    public class Option
    {
        private readonly OptionDelta delta;
        private readonly OptionLength length;
        private readonly byte[] value;

        public Option(OptionDelta delta, OptionLength length, byte[] value)
        {
            if (value.Length != length)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value.Length, $"Expected {length} bytes for option value (Found {value.Length} bytes).");
            }

            this.delta = delta;
            this.length = length;
            this.value = value;
        }
    }
}