// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using Common.Extensions;

    /// <summary>
    /// Represents a <see cref="CoapOption"/> as a opaque sequence of bytes. This means the value of that <see cref="CoapOption"/>
    /// is a opaque sequence of bytes.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.Messages.Options.CoapOption" />
    /// <seealso cref="System.IEquatable{WorldDirect.CoAP.Messages.Options.OpaqueOptionFormat}" />
    public abstract class OpaqueOption : CoapOption<ReadOnlyMemory<byte>>
    {
        private const ushort MIN_LENGTH = 0;

        protected OpaqueOption(ushort number, ReadOnlyMemory<byte> value, uint minLength, uint maxLength, bool isRepeatable)
            : base(number, value, minLength, maxLength, isRepeatable)
        {
            if (value.Length < minLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value.Length, $"Specified opaque option is to small (actual size: {value.Length}; allowed minimum size: {minLength}).");
            }

            if (value.Length > maxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value.Length, $"Specified opaque option is to big (actual size: {value.Length}; allowed maximum size: {maxLength}).");
            }
        }

        protected OpaqueOption(ushort number, ReadOnlyMemory<byte> value, uint maxlength, bool isRepeatable)
            : this(number, value, MIN_LENGTH, maxlength, isRepeatable)
        {
        }

        public override string ToString() => $"{base.ToString()}: {this.Value.Span.ToString(' ')}";
    }
}
