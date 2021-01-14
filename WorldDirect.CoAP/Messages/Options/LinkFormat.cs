namespace WorldDirect.CoAP.Messages.Options
{
    public sealed class LinkFormat : ContentFormat
    {
        public LinkFormat()
            : base(40)
        {
        }

        public override string MediaType => "application/link-format";
    }
}