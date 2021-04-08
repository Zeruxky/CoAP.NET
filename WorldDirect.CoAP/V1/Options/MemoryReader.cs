namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Buffers.Binary;
    using System.Text;
    using Common.Extensions;

    public static class MemoryReader
    {
        public static ReadOnlyMemory<byte> ReadBytesBigEndian(ReadOnlyMemory<byte> value, int from, int to)
        {
            return ReadBytes(value, ByteOrder.BigEndian, from, to);
        }

        public static ReadOnlyMemory<byte> ReadBytesBigEndian(ReadOnlyMemory<byte> value, int from)
        {
            return ReadBytesBigEndian(value, from, value.Length);
        }

        public static ReadOnlyMemory<byte> ReadBytesBigEndian(ReadOnlyMemory<byte> value)
        {
            return ReadBytesBigEndian(value, 0);
        }

        public static ReadOnlyMemory<byte> ReadBytesLittleEndian(ReadOnlyMemory<byte> value, int from, int to)
        {
            return ReadBytes(value, ByteOrder.LittleEndian, from, to);
        }

        public static ReadOnlyMemory<byte> ReadBytesLittleEndian(ReadOnlyMemory<byte> value, int from)
        {
            return ReadBytesLittleEndian(value, from, value.Length);
        }

        public static ReadOnlyMemory<byte> ReadBytesLittleEndian(ReadOnlyMemory<byte> value)
        {
            return ReadBytesLittleEndian(value, 0);
        }

        public static uint ReadUInt32BigEndian(ReadOnlyMemory<byte> value)
        {
            return ReadUInt32(value, ByteOrder.BigEndian);
        }

        public static uint ReadUInt32LittleEndian(ReadOnlyMemory<byte> value)
        {
            return ReadUInt32(value, ByteOrder.LittleEndian);
        }

        public static string ReadUtf8String(ReadOnlyMemory<byte> value)
        {
            return ReadString(value, Encoding.UTF8);
        }

        public static string ReadString(ReadOnlyMemory<byte> value, Encoding encoding)
        {
            return encoding.GetString(value.Span);
        }

        /// <summary>
        /// Reads a <see cref="uint"/> value of the specified <see cref="Span{T}"/> of <see cref="byte"/>s depending on the specified <see cref="ByteOrder"/>. 
        /// </summary>
        /// <param name="value">The <see cref="Span{T}"/> from which should be read.</param>
        /// <param name="byteOrder">The byte order of the read value.</param>
        /// <returns>The read <see cref="uint"/> value depending on the given <paramref name="byteOrder"/>.</returns>
        private static uint ReadUInt32(ReadOnlyMemory<byte> value, ByteOrder byteOrder)
        {
            var span = value.Span;
            if (span.Length < 4)
            {
                span = span.Align(4);
            }

            var readValue = BinaryPrimitives.ReadUInt32BigEndian(span);
            if (byteOrder == ByteOrder.LittleEndian)
            {
                readValue = BinaryPrimitives.ReverseEndianness(readValue);
            }

            return readValue;
        }

        private static ReadOnlyMemory<byte> ReadBytes(ReadOnlyMemory<byte> value, ByteOrder byteOrder, int from, int to)
        {
            if (byteOrder == ByteOrder.BigEndian && BitConverter.IsLittleEndian)
            {
                value.Reverse();
            }

            if (byteOrder == ByteOrder.LittleEndian && !BitConverter.IsLittleEndian)
            {
                value.Reverse();
            }

            return value.Slice(from, to);
        }
    }
}