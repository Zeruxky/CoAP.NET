namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Text;

    public class UriPath : StringOptionFormat
    {
        public const ushort NUMBER = 11;

        public UriPath(string value)
            : base(NUMBER, value, 0, 255)
        {
        }
    }

    public class UriPathFactory : IOptionFactory
    {
        /// <inheritdoc />
        public CoapOption Create(OptionData src)
        {
            return new UriPath(src.StringValue);
        }

        /// <inheritdoc />
        public int Number => UriPath.NUMBER;
    }
}
