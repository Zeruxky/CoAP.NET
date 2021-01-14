namespace WorldDirect.CoAP.Messages.Options
{
    public sealed class XmlFormat : ContentFormat
    {
        public XmlFormat()
            : base(41)
        {
        }

        public override string MediaType => "application/xml";
    }
}