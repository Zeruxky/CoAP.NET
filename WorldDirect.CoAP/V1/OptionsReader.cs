// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1
{
    using System;
    using System.Buffers.Binary;
    using System.Collections.Generic;
    using System.Linq;
    using WorldDirect.CoAP.Codes.Common;
    using WorldDirect.CoAP.Common;
    using WorldDirect.CoAP.V1.Options;

    /// <summary>
    /// Provides functionality to read a set of <see cref="CoapOption"/>s from a provided <see cref="ReadOnlyMemory{T}"/> that are compliant to RFC 7252.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.IReader{WorldDirect.CoAP.V1.ReadOnlyOptionCollection}" />
    public class OptionsReader : IReader<ReadOnlyOptionCollection>
    {
        private const byte MASK_DELTA = 0xF0;
        private const byte MASK_LENGTH = 0x0F;
        private const byte PAYLOAD_MARKER = 0xFF;

        private readonly SortedDictionary<int, IOptionFactory> factories;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsReader"/> class.
        /// </summary>
        /// <param name="factories">The set of <see cref="IOptionFactory"/>s for constructing <see cref="CoapOption"/>s.</param>
        public OptionsReader(IEnumerable<IOptionFactory> factories)
        {
            this.factories = new SortedDictionary<int, IOptionFactory>(factories.ToDictionary(f => f.Number));
        }

        /// <inheritdoc />
        public int Read(ReadOnlyMemory<byte> value, out ReadOnlyOptionCollection result)
        {
            var options = new OptionCollection();
            var offset = 0;
            var previousNumber = 0;
            while (OptionsReader.CanReadFurther(value.Span.Slice(offset)))
            {
                var optionData = OptionsReader.ReadOptionData(previousNumber, value.Slice(offset), out var bytesConsumed);
                previousNumber = optionData.Number;
                offset += bytesConsumed;

                var factory = this.factories.ContainsKey(optionData.Number)
                    ? this.factories[optionData.Number]
                    : new UnrecognizedOptionFactory();
                var option = factory.Create(optionData);

                options.Add(option);
            }

            result = new ReadOnlyOptionCollection(options);
            return offset;
        }

        private static OptionData ReadOptionData(int previousNumber, ReadOnlyMemory<byte> src, out int bytesConsumed)
        {
            var delta = OptionsReader.ReadDelta(src);
            var length = OptionsReader.ReadLength(src, delta.Size);

            var valueOffset = 1 + delta.Size + length.Size;

            bytesConsumed = valueOffset + length.Value;
            return new OptionData((ushort)previousNumber, delta.Value, length.Value, src.Slice(valueOffset, length.Value));
        }

        private static OptionsLength ReadLength(ReadOnlyMemory<byte> value, byte readDeltaBytes)
        {
            var length = (UInt4)(value.Span[0] & MASK_LENGTH);
            var startOfExtendedLength = readDeltaBytes + 1;
            if (length == 15)
            {
                throw new MessageFormatErrorException("Length value of 15 is reserved for future use.");
            }

            if (length == 14)
            {
                var extended = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(value.Span.Slice(startOfExtendedLength, 2)) + 269);
                return new OptionsLength(extended, 2);
            }

            if (length == 13)
            {
                var extended = (ushort)(value.Span[startOfExtendedLength] + 13);
                return new OptionsLength(extended, 1);
            }

            return new OptionsLength(length, 0);
        }

        private static OptionsDelta ReadDelta(ReadOnlyMemory<byte> value)
        {
            var delta = (UInt4)((value.Span[0] & MASK_DELTA) >> 4);
            var length = (UInt4)(value.Span[0] & MASK_LENGTH);
            if (delta == 15 && length != 15)
            {
                throw new MessageFormatErrorException("Delta value of 15 is reserved for payload marker.");
            }

            if (delta == 14)
            {
                var extended = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(value.Span.Slice(1, 2)) + 269);
                return new OptionsDelta(extended, 2);
            }

            if (delta == 13)
            {
                var extended = (ushort)(value.Span[1] + 13);
                return new OptionsDelta(extended, 1);
            }

            return new OptionsDelta(delta, 0);
        }

        private static bool CanReadFurther(ReadOnlySpan<byte> value)
        {
            return value.Length != 0 && value[0] != PAYLOAD_MARKER;
        }
    }
}
