namespace WorldDirect.CoAP.Messages.Options
{
    public sealed class JsonFormat : ContentFormat
    {
        public JsonFormat()
            : base(50)
        {
        }

        public override string MediaType => "application/json";
    }
}