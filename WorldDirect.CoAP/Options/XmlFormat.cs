namespace WorldDirect.CoAP.Options
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