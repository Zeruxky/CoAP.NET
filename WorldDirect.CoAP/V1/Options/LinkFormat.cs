namespace WorldDirect.CoAP.V1.Options
{
    /// <summary>
    /// Represents the link-format as a <see cref="ContentFormat"/>.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.ContentFormat" />
    public sealed class LinkFormat : ContentFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkFormat"/> class.
        /// </summary>
        public LinkFormat()
            : base(40, CoapMediaTypeNames.Application.LinkFormat)
        {
        }
    }
}
