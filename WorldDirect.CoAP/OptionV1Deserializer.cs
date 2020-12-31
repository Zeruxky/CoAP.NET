namespace WorldDirect.CoAP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WorldDirect.CoAP.Common.Extensions;

    public static class OptionV1Deserializer
    {
        private const byte MASK_LENGTH = 0x0F;
        private const byte MASK_DELTA = 0xF0;

        public static bool CanDeserialize(Version version)
        {
            return version.Equals(Version.V1);
        }

        public static IEnumerable<IOption> Deserialize(byte[] value, OptionsRegistry registry)
        {
            var options = new List<IOption>();
            var lastIndex = 0;
            IOption previousOption = null;
            while (value[lastIndex] != 0xFF)
            {
                var result = OptionV1Deserializer.ParseOption(value.Slice(lastIndex), previousOption, lastIndex, registry);
                previousOption = result.Option;
                lastIndex = result.LastIndex;
                options.Add(result.Option);
            }

            return options.OrderBy(o => o.Number);
        }

        private static OptionDeserializerResult ParseOption(byte[] value, IOption previousOption, int lastIndex, OptionsRegistry registry)
        {
            var delta = (ushort)((value[0] & MASK_DELTA) >> 4);
            var length = (ushort)(value[0] & MASK_LENGTH);
            if (delta == 15 && length != 15)
            {
                throw new MessageFormatError("Delta is not allowed to be 15 (0xFF), because it is reserved for payload marker.");
            }

            var currentIndex = 1;
            if (delta == 14)
            {
                delta = (ushort)(BitConverter.ToUInt16(value, 1) - 269);
                currentIndex = 3;
            }

            if (delta == 13)
            {
                delta = (ushort)(value[1] - 13);
                currentIndex = 2;
            }

            if (length == 15)
            {
                throw new MessageFormatError("Length 15 (0xFF) is reserved for future use.");
            }

            if (length == 14)
            {
                length = (ushort)(BitConverter.ToUInt16(value, currentIndex) - 269);
                currentIndex += 2;
            }

            if (length == 13)
            {
                length = (ushort)(value[currentIndex] - 13);
                currentIndex += 1;
            }

            var optionValue = value.Slice(currentIndex, length);
            lastIndex += currentIndex + length;
            var number = previousOption == null
                ? delta
                : (ushort)(previousOption.Number + delta);
            var option = registry.CreateOption(number, optionValue);
            return new OptionDeserializerResult(option, lastIndex);
        }

        private class OptionDeserializerResult
        {
            public OptionDeserializerResult(IOption option, int lastIndex)
            {
                this.Option = option;
                this.LastIndex = lastIndex;
            }

            public IOption Option { get; }

            public int LastIndex { get; }
        }
    }
}
