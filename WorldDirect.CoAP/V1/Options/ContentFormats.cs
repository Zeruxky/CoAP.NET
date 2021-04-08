namespace WorldDirect.CoAP.V1.Options
{
    /// <summary>
    /// Contains all <see cref="ContentFormat"/>s for CoAP.
    /// </summary>
    public sealed class ContentFormats
    {
        /// <summary>
        /// The <see cref="ContentFormat"/> for text plain.
        /// </summary>
        public static readonly ContentFormat TextPlainContentFormat = new TextPlainContentFormat();

        /// <summary>
        /// the <see cref="ContentFormat"/> for link-format.
        /// </summary>
        public static readonly ContentFormat LinkContentFormat = new LinkFormat();

        /// <summary>
        /// The <see cref="ContentFormat"/> for XML.
        /// </summary>
        public static readonly ContentFormat XmlContentFormat = new XmlFormat();

        /// <summary>
        /// The <see cref="ContentFormat"/> for octet stream.
        /// </summary>
        public static readonly ContentFormat OctetStreamContentFormat = new OctetStreamFormat();

        /// <summary>
        /// The <see cref="ContentFormat"/> for EXI.
        /// </summary>
        public static readonly ContentFormat ExiContentFormat = new ExiFormat();

        /// <summary>
        /// The <see cref="ContentFormat"/> for JSON.
        /// </summary>
        public static readonly ContentFormat JsonContentFormat = new JsonFormat();
    }
}
