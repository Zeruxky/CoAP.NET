namespace WorldDirect.CoAP.Options
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