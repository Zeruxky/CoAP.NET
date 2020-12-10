namespace WorldDirect.CoAP
{
    using System;

    public class OptionLength
    {
        private const byte MASK = 0x0F;
        private readonly byte[] value;

        public OptionLength(byte[] value)
        {
            if (value.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value.Length, $"Expected at least one byte (Found {value.Length} bytes).");
            }

            this.value = value;
        }

        public static implicit operator ushort(OptionLength length)
        {
            var tmp = (UInt4)(length.value[0] & MASK);
            if (tmp <= 12)
            {
                return tmp;
            }

            if (tmp == 13)
            {
                return (ushort)(length.value[1] - 13);
            }

            if (tmp == 14)
            {
                return (ushort)(BitConverter.ToUInt16(length.value, 1) - 269);
            }

            throw new MessageFormatError("Value 15 (0xFF) is reserved for future use.", nameof(OptionLength));
        }

        public override string ToString()
        {
            return ((ushort)this).ToString("D");
        }
    }
}