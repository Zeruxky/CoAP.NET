namespace WorldDirect.CoAP.Messages.Options
{
    public class MaxAge : UIntOptionFormat
    {
        public MaxAge(uint value)
            : base(value)
        {
        }

        public override ushort Number => 14;

        public override string ToString()
        {
            return $"Max-Age ({this.Number}): {this.Value}";
        }
    }
}
