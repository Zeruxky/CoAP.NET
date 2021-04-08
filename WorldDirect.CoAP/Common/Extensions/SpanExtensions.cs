namespace WorldDirect.CoAP.Common.Extensions
{
    using System;
    using System.Buffers.Binary;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    public static class SpanExtensions
    {
        /// <summary>
        /// Aligns the specified <see cref="ReadOnlySpan{T}"/> to the specified amount of <typeparamref name="T"/>s.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlySpan{T}"/> that should be aligned to the specified <paramref name="size"/>.</param>
        /// <param name="size">The size to which the specified <paramref name="value"/> should be aligned.</param>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> that is aligned to the specified <paramref name="size"/> and holds the content of the <paramref name="value"/>.</returns>
        /// <remarks>
        /// 'Align' means that the given <see cref="Span{T}"/> will be filled up with default values of <typeparamref name="T"/> to the specified <paramref name="size"/>.
        /// </remarks>
        public static ReadOnlySpan<T> Align<T>(this ReadOnlySpan<T> value, int size)
        {
            // Initialize buffer with the specified size and set every item to zero (0) as default.
            var buffer = new T[size];

            // Check if the span fits into the buffer
            if (value.Length > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value.Length, "The specified value does not fit.");
            }

            // Fill the content of the Span into the buffer.
            var offset = size - value.Length;
            for (int i = 0; i < value.Length; i++)
            {
                buffer[i + offset] = value[i];
            }

            return buffer;
        }

        public static ReadOnlySpan<byte> RemoveLeadingZeros(this ReadOnlySpan<byte> value)
        {
            var index = 0;
            while (value[index] != 0)
            {
                index++;
            }

            return index == 0
                ? value
                : value.Slice(0, index);
        }

        public static Span<byte> RemoveLeadingZeros(this Span<byte> value)
        {
            var index = 0;
            while (value[index] != 0)
            {
                index++;
            }

            return index == 0
                ? value
                : value.Slice(0, index);
        }

        public static string ToString(this Span<byte> value, char separator)
        {
            var builder = new StringBuilder();
            foreach (var item in value)
            {
                builder.Append(item.ToString("X"));
                builder.Append(separator);
            }

            return builder.ToString();
        }

        public static string ToString(this ReadOnlySpan<byte> value, char separator)
        {
            var builder = new StringBuilder();
            foreach (var item in value)
            {
                builder.Append(item.ToString("X"));
                builder.Append(separator);
            }

            return builder.ToString();
        }
    }
}
