namespace WorldDirect.CoAP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class OptionV1Serializer
    {
        public static bool CanSerialize(Version version)
        {
            return version.Equals(Version.V1);
        }

        public static byte[] Serialize(IEnumerable<IOption> options)
        {
            var bytes = new List<byte>();
            IOption previousOption = null;
            foreach (var option in options.OrderBy(o => o.Number))
            {
                bytes.AddRange(OptionV1Serializer.GetBytes(option, previousOption));
                previousOption = option;
            }

            return bytes.ToArray();
        }

        public static byte[] Serialize(IOption option)
        {
            var bytes = OptionV1Serializer.GetBytes(option, null);
            return bytes.ToArray();
        }

        private static IEnumerable<byte> GetBytes(IOption currentOption, IOption previousOption)
        {
            var delta = previousOption == null
                ? currentOption.Number
                : currentOption.Number - previousOption.Number;
            var deltaExtended = (ushort)0;
            if (delta > 12 && delta <= (byte.MaxValue - 13))
            {
                deltaExtended = (byte)(currentOption.Number + 13);
                delta = 13;
            }

            if (delta > byte.MaxValue && delta <= (ushort.MaxValue - 269))
            {
                deltaExtended = (ushort)(currentOption.Number + 269);
                delta = 14;
            }

            delta <<= 4;

            var length = currentOption.RawValue.Length;
            var lengthExtended = (ushort)0;
            if (length > 12 && length <= (byte.MaxValue - 13))
            {
                lengthExtended = (byte)(length + 13);
                length = 13;
            }

            if (length > byte.MaxValue && length <= (ushort.MaxValue - 269))
            {
                lengthExtended = (ushort)(length + 269);
                length = 14;
            }

            var serialized = Enumerable.Empty<byte>().Append((byte)(delta | length));
            if (deltaExtended != 0)
            {
                if (deltaExtended <= byte.MaxValue)
                {
                    serialized = serialized.Append((byte)deltaExtended);
                }

                if (deltaExtended > byte.MaxValue)
                {
                    serialized = serialized.Concat(BitConverter.GetBytes(deltaExtended));
                }
            }

            if (lengthExtended != 0)
            {
                if (lengthExtended <= byte.MaxValue)
                {
                    serialized = serialized.Append((byte)lengthExtended);
                }

                if (lengthExtended > byte.MaxValue)
                {
                    serialized = serialized.Concat(BitConverter.GetBytes(lengthExtended));
                }
            }

            serialized = serialized.Concat(currentOption.RawValue);
            return serialized;
        }
    }
}
