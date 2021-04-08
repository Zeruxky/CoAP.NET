// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Text;

    /// <summary>
    /// Represents a <see cref="CoapOption"/> in <see cref="string"/> format. This means the value of that <see cref="CoapOption"/>
    /// represents a unicode <see cref="string"/> that is encoded using UTF-8 in Net-Unicode form.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.Messages.Options.CoapOption" />
    /// <seealso cref="System.IEquatable{WorldDirect.CoAP.Messages.Options.StringOptionFormat}" />
    public abstract class StringOption : CoapOption<string>
    {
        private const ushort MIN_LENGTH = 0;

        protected StringOption(ushort number, string value, uint minLength, uint maxLength, bool isRepeatable)
            : base(number, value, minLength, maxLength, isRepeatable)
        {
            var bytes = Encoding.UTF8.GetByteCount(value);
            if (bytes < minLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value.Length, $"Specified string option is too small (actual size: {bytes}; allowed minimum size: {minLength}).");
            }

            if (bytes > maxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value.Length, $"Specified string option is too large (actual size: {bytes}; allowed maximum size: {maxLength}).");
            }
        }

        protected StringOption(ushort number, string value, uint maxLength, bool isRepeatable)
            : this(number, value, MIN_LENGTH, maxLength, isRepeatable)
        {
        }

        protected StringOption(ushort number, ReadOnlyMemory<byte> value, uint minLength, uint maxLength, bool isRepeatable)
            : this(number, MemoryReader.ReadUtf8String(value), minLength, maxLength, isRepeatable)
        {
        }

        protected StringOption(ushort number, ReadOnlyMemory<byte> value, uint maxLength, bool isRepeatable)
            : this(number, value, MIN_LENGTH, maxLength, isRepeatable)
        {
        }
    }
}
