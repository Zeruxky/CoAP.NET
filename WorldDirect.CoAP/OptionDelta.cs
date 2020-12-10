namespace WorldDirect.CoAP
{
    using System;

    public class OptionDelta
    {
        private const byte MASK = 0xF0;
        private readonly byte[] value;

        public OptionDelta(byte[] value)
        {
            if (value.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value.Length, $"Expected at least one byte (Found {value.Length} bytes).");
            }

            this.value = value;
        }

        public static implicit operator ushort(OptionDelta delta)
        {
            var tmp = (UInt4)((delta.value[0] & MASK) >> 4);
            if (tmp <= 12)
            {
                return tmp;
            }

            if (tmp == 13)
            {
                return (ushort)(delta.value[1] - 13);
            }

            if (tmp == 14)
            {
                return (ushort)(BitConverter.ToUInt16(delta.value, 1) - 269);
            }

            throw new MessageFormatError("Value 15 (0xFF) is reserved for payload marker.", nameof(OptionDelta));
        }

        public override string ToString()
        {
            return ((ushort)this).ToString("D");
        }
    }
}