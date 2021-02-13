namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Text;

    public class UriPath : StringOptionFormat
    {
        public UriPath(string value)
            : base(value)
        {
            if (value.Length > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value for Uri-Path can only be in range of 0 - 255 characters.");
            }
        }

        public override ushort Number => 11;
    }

    public class UriPathFactory : IOptionFactory
    {
        /// <inheritdoc />
        public CoapOption Create(OptionData src)
        {
            return new UriPath(Encoding.UTF8.GetString(src.Value.ToArray()));
        }

        /// <inheritdoc />
        public int Number => 11;
    }
}
