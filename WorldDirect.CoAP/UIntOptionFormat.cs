namespace WorldDirect.CoAP
{
    using System;

    public abstract class UIntOptionFormat : IOption
    {
        protected UIntOptionFormat(uint value)
        {
            this.RawValue = BitConverter.GetBytes(value);
        }

        public abstract ushort Number { get; }

        public byte[] RawValue { get; set; }

        public uint Value => BitConverter.ToUInt32(this.RawValue, 0);
    }
}