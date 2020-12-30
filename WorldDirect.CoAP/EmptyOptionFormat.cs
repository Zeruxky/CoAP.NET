namespace WorldDirect.CoAP
{
    using System;
    using System.Linq;

    public abstract class EmptyOptionFormat : IOption
    {
        public abstract ushort Number { get; }

        public byte[] RawValue
        {
            get
            {
                return Enumerable.Empty<byte>().ToArray();
            }

            set
            {
                if (value.Length > 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "Empty option can not set a raw value.");
                }
            }
        }
    }
}
