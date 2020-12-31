namespace WorldDirect.CoAP
{
    public abstract class OpaqueOptionFormat : IOption
    {
        protected OpaqueOptionFormat(byte[] value)
        {
            this.RawValue = value;
        }

        public abstract ushort Number { get; }

        public byte[] RawValue { get; set; }
    }
}
