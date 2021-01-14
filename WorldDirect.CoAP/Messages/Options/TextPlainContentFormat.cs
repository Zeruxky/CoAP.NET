namespace WorldDirect.CoAP.Messages.Options
{
    public sealed class TextPlainContentFormat : ContentFormat
    {
        public TextPlainContentFormat()
            : base(0)
        {
        }

        public override string MediaType => "text/plain; charset=utf-8";
    }
}