namespace WorldDirect.CoAP.V1.Options
{
    using System.Text;

    /// <summary>
    /// Represents JSON as a <see cref="ContentFormat"/> defined by RFC 7252.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.ContentFormat" />
    public sealed class JsonFormat : ContentFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFormat"/> class.
        /// </summary>
        public JsonFormat()
            : base(50, CoapMediaTypeNames.Application.Json)
        {
        }
    }
}
