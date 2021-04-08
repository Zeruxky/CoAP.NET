// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    /// <summary>
    /// Represents a <see cref="CoapOption"/> with an <see cref="uint"/> as value.
    /// </summary>
    public abstract class UIntOption : CoapOption<uint>
    {
        private const ushort MIN_LENGTH = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIntOption"/> class.
        /// </summary>
        /// <param name="number">The number of the <see cref="CoapOption{T}"/>.</param>
        /// <param name="value">The <see cref="uint"/> value of that <see cref="CoapOption{T}"/>.</param>
        /// <param name="minLength">The minimum allowed length for the specified <paramref name="value"/> in bytes.</param>
        /// <param name="maxLength">The maximum allowed length for the specified <paramref name="value"/> in bytes.</param>
        /// <param name="isRepeatable">If set to <see langword="true"/> the <see cref="CoapOption{T}"/> can be appear
        /// multiple times in a <see cref="OptionCollection"/>; Otherwise <see langword="false"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws if the length of the <paramref name="value"/> in bytes is smaller than the <paramref name="minLength"/>.
        /// or
        /// throws if the length of the <paramref name="value"/> in bytes is greater than the <paramref name="maxLength"/>.
        /// </exception>
        protected UIntOption(ushort number, uint value, uint minLength, uint maxLength, bool isRepeatable)
            : base(number, value, minLength, maxLength, isRepeatable)
        {
            var usedBytes = UIntOption.GetUsedBytes(value);
            if (usedBytes < minLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), usedBytes, $"Specified uint option is to small (actual size: {usedBytes}; allowed minimum size: {minLength}).");
            }

            if (usedBytes > maxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), usedBytes, $"Specified uint option is to big (actual size: {usedBytes}; allowed maximum size: {maxLength}).");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIntOption"/> class.
        /// </summary>
        /// <param name="number">The number of the <see cref="CoapOption{T}"/>.</param>
        /// <param name="value">The <see cref="uint"/> value of that <see cref="CoapOption{T}"/>.</param>
        /// <param name="maxLength">The maximum allowed length for the specified <paramref name="value"/> in bytes.</param>
        /// <param name="isRepeatable">If set to <see langword="true"/> the <see cref="CoapOption{T}"/> can be appear
        /// multiple times in a <see cref="OptionCollection"/>; Otherwise <see langword="false"/>.</param>
        protected UIntOption(ushort number, uint value, uint maxLength, bool isRepeatable)
            : this(number, value, MIN_LENGTH, maxLength, isRepeatable)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIntOption"/> class.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}"/> that contains the value for that <see cref="CoapOption{T}"/>.</param>
        /// <param name="minLength">The minimum allowed length for the specified <paramref name="value"/> in bytes.</param>
        /// <param name="maxLength">The maximum allowed length for the specified <paramref name="value"/> in bytes.</param>
        /// <param name="isRepeatable">If set to <see langword="true"/> the <see cref="CoapOption{T}"/> can be appear
        /// multiple times in a <see cref="OptionCollection"/>; Otherwise <see langword="false"/>.</param>
        protected UIntOption(ushort number, ReadOnlyMemory<byte> value, uint minLength, uint maxLength, bool isRepeatable)
            : this(number, MemoryReader.ReadUInt32BigEndian(value), minLength, maxLength, isRepeatable)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIntOption"/> class.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}"/> that contains the value for that <see cref="CoapOption{T}"/>.</param>
        /// <param name="maxLength">The maximum allowed length for the specified <paramref name="value"/> in bytes.</param>
        /// <param name="isRepeatable">If set to <see langword="true"/> the <see cref="CoapOption{T}"/> can be appear
        /// multiple times in a <see cref="OptionCollection"/>; Otherwise <see langword="false"/>.</param>
        protected UIntOption(ushort number, ReadOnlyMemory<byte> value, uint maxLength, bool isRepeatable)
            : this(number, value, MIN_LENGTH, maxLength, isRepeatable)
        {
        }

        private static int GetUsedBytes(uint value)
        {
            if (value == 0)
            {
                return 0;
            }

            var exponent = 0;
            while (value <= Math.Pow(2, exponent) || value >= Math.Pow(2, exponent + 1))
            {
                exponent++;
            }

            var bytesUsed = exponent / 8d;
            return Convert.ToInt32(Math.Round(bytesUsed, MidpointRounding.AwayFromZero));
        }
    }
}
