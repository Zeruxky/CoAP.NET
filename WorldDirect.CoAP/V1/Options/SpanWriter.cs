namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Buffers.Binary;
    using System.Text;
    using Common.Extensions;

    public static class SpanWriter
    {
        public static Span<byte> WriteUInt32BigEndian(uint value)
        {
            return WriteUInt32(value, ByteOrder.BigEndian);
        }

        public static Span<byte> WriteUInt32LittleEndian(uint value)
        {
            return WriteUInt32(value, ByteOrder.LittleEndian);
        }

        public static Span<byte> WriteUtf8String(string value)
        {
            return WriteString(value, Encoding.UTF8);
        }

        public static Span<byte> WriteString(string value, Encoding encoding)
        {
            var charSpan = value.AsSpan();
            var byteCount = encoding.GetByteCount(charSpan);
            var buffer = new Span<byte>(new byte[byteCount]);
            encoding.GetBytes(charSpan, buffer);
            return buffer.RemoveLeadingZeros();
        }

        private static Span<byte> WriteUInt32(uint value, ByteOrder byteOrder)
        {
            var span = new Span<byte>(new byte[4]);
            BinaryPrimitives.WriteUInt32LittleEndian(span, value);
            if (byteOrder == ByteOrder.BigEndian)
            {
                span.Reverse();
            }

            return span.RemoveLeadingZeros();
        }
    }
}