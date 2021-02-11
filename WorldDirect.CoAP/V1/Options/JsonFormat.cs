namespace WorldDirect.CoAP.V1.Options
{
    public sealed class JsonFormat : ContentFormat
    {
        public JsonFormat()
            : base(50, "application/json")
        {
        }
    }
}
