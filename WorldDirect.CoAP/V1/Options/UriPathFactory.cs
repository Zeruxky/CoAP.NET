namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriPathFactory : IOptionFactory
    {
        /// <inheritdoc />
        public CoapOption Create(OptionData src)
        {
            if (src.Number != UriPath.NUMBER)
            {
                throw new ArgumentException($"Option data number {src.Number} is not valid for Uri-Path factory.");
            }

            return new UriPath(src.Value);
        }

        /// <inheritdoc />
        public int Number => UriPath.NUMBER;
    }
}
