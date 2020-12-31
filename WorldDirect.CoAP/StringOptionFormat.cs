namespace WorldDirect.CoAP
{
    using System.Text;

    public abstract class StringOptionFormat : IOption
    {
        private readonly Encoding encoding;

        protected StringOptionFormat(byte[] value, Encoding encoding)
        {
            this.RawValue = value;
            this.encoding = encoding;
        }

        protected StringOptionFormat(byte[] value)
            : this(Encoding.UTF8.GetString(value))
        {
        }

        protected StringOptionFormat(string value, Encoding encoding)
            : this(encoding.GetBytes(value), encoding)
        {
        }

        protected StringOptionFormat(string value)
            : this(value, Encoding.UTF8)
        {
        }

        public abstract ushort Number { get; }

        public byte[] RawValue { get; set; }

        public string Value => this.encoding.GetString(this.RawValue);
    }
}
