namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriPortFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            if (src.Number != UriPort.NUMBER)
            {
                throw new ArgumentException($"Option data number {src.Number} is not valid for Uri-Port factory.");
            }

            return new UriPort(src.Value);
        }

        public int Number => UriPort.NUMBER;
    }
}
