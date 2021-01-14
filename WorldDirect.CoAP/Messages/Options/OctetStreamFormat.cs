namespace WorldDirect.CoAP.Messages.Options
{
    public sealed class OctetStreamFormat : ContentFormat
    {
        public OctetStreamFormat()
            : base(42)
        {
        }

        public override string MediaType => "application/octet-stream";
    }
}