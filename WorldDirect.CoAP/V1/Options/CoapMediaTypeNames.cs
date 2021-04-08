namespace WorldDirect.CoAP.V1.Options
{
    /// <summary>
    /// Contains all media types that defined for CoAP.
    /// </summary>
    public static class CoapMediaTypeNames
    {
        /// <summary>
        /// Contains all media types for text category.
        /// </summary>
        public static class Text
        {
            /// <summary>
            /// The media type for text/plain; charset=utf-8.
            /// </summary>
            public const string TextPlainUtf8 = "text/plain; charset=utf-8";
        }

        /// <summary>
        /// Contains all media types for application category.
        /// </summary>
        public static class Application
        {
            /// <summary>
            /// The media type for application/link-format.
            /// </summary>
            public const string LinkFormat = "application/link-format";

            /// <summary>
            /// The media type for application/xml.
            /// </summary>
            public const string Xml = "application/xml";

            /// <summary>
            /// The media type for application/octet-stream.
            /// </summary>
            public const string OctetStream = "application/octet-stream";

            /// <summary>
            /// The media type for application/exi.
            /// </summary>
            public const string Exi = "application/exi";

            /// <summary>
            /// The media type for application/json.
            /// </summary>
            public const string Json = "application/json";
        }
    }
}
