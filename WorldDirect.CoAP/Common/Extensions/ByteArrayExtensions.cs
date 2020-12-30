namespace WorldDirect.CoAP.Common.Extensions
{
    using System;
    using System.Linq;

    public static class ByteArrayExtensions
    {
        public static byte[] Slice(this byte[] value, int start)
        {
            return value.Slice(start, value.Length - start);
        }

        public static byte[] Slice(this byte[] value, int start, int length)
        {
            if (start < 0 || start > value.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(start), value, $"Start index must be in range of 0 - {value.Length}.");
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, "Length can not be negative.");
            }

            if (start + length > value.Length)
            {
                throw new InvalidOperationException("Can not read beyond the given byte array.");
            }

            var buffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = value[start + i];
            }

            return buffer;
        }

        public static byte[] Append(this byte[] array, byte value)
        {
            var result = array.ToList();
            result.Add(value);
            return result.ToArray();
        }
    }
}
