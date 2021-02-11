namespace WorldDirect.CoAP.V1.Options
{
    public class Size1 : UIntOptionFormat
    {
        public Size1(uint value)
            : base(value)
        {
        }

        public override ushort Number => 60;

        public override string ToString()
        {
            return $"Size1 ({this.Number}): {this.Value}";
        }
    }
}