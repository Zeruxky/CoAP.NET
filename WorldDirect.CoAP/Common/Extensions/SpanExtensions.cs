namespace WorldDirect.CoAP.Common.Extensions
{
    using System;

    public static class SpanExtensions
    {
        /// <summary>
        /// Aligns the specified <see cref="ReadOnlySpan{T}"/> to the specified <paramref name="size"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlySpan{T}"/> that should be aligned to the specified <paramref name="size"/>.</param>
        /// <param name="size">The size of the resulting <see cref="ReadOnlySpan{T}"/>.</param>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> that is aligned to the specified <paramref name="size"/> and holds the content of the <see cref="value"/>.</returns>
        /// <remarks>
        /// 'Align' means that a new byte array of the specified <paramref name="size"/> will be created and filled with zeros (0) at default.
        /// In the next step, the newly created byte array will be filled with the content of the specified <paramref name="value"/>.
        /// </remarks>
        public static ReadOnlySpan<byte> AlignByteArray(this ReadOnlySpan<byte> value, int size)
        {
            // Initialize buffer with the specified size and set every item to zero (0) as default.
            var buffer = new byte[size];

            // Fill the content of the Span into the buffer.
            int index = 0;
            foreach (var item in value)
            {
                buffer[index++] = item;
            }

            return buffer;
        }

        public static ReadOnlySpan<byte> Reverse(this ReadOnlySpan<byte> value)
        {
            var index = value.Length;
            var buffer = new byte[value.Length];
            foreach (var item in value)
            {
                buffer[--index] = item;
            }

            return buffer;
        }

        public static Span<byte> Reverse(this Span<byte> value)
        {
            var index = value.Length;
            var buffer = new byte[value.Length];
            foreach (var item in value)
            {
                buffer[--index] = item;
            }

            return buffer;
        }
    }
}