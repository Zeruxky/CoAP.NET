namespace WorldDirect.CoAP.Common.Extensions
{
    using System;

    /// <summary>
    /// Contains method extensions for objects of type <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public static class ReadOnlySpanExtensions
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
        public static ReadOnlySpan<byte> Align(this ReadOnlySpan<byte> value, int size)
        {
            // Initialize buffer with the specified size and set every item to zero (0) as default.
            var buffer = new byte[size];

            // Check if the span fits into the buffer
            if (value.Length > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "Can not align the specified span to the given size.");
            }

            // Fill the content of the Span into the buffer.
            int offset = size - value.Length;
            for (int i = 0; i < value.Length; i++)
            {
                buffer[i + offset] = value[i];
            }

            return buffer;
        }

        /// <summary>
        /// Reverses the content of the specified <see cref="ReadOnlySpan{T}"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlySpan{T}"/> that should be reversed.</param>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> that contains the reversed content of the specified <paramref name="value"/>.</returns>
        /// <remarks>
        /// This operation leads into a memory allocation of the specified <paramref name="value"/> for reversing it.
        /// </remarks>
        public static ReadOnlySpan<byte> Reverse(this ReadOnlySpan<byte> value)
        {
            var offset = value.Length - 1;
            var buffer = new byte[value.Length];
            for (int i = offset; i >= 0; i--)
            {
                buffer[i] = value[offset - i];
            }

            return buffer;
        }
    }
}
