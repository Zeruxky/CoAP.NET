namespace WorldDirect.CoAP.V1.Options
{
    public class Size1 : UIntOptionFormat
    {
        public const ushort NUMBER = 60;

        public Size1(uint value)
            : base(NUMBER, value, 0, 4)
        {
        }
    }

    public class Size1Factory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new Size1(src.UIntValue);
        }

        public int Number => Size1.NUMBER;
    }
}
