namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class AcceptFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            if (src.Number != Accept.NUMBER)
            {
                throw new ArgumentException($"Option data number {src.Number} is not valid for Uri-Path factory.");
            }

            return new Accept(src.Value);
        }

        public int Number => Accept.NUMBER;
    }
}
