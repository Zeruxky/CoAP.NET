namespace WorldDirect.CoAP.V1.Options
{
    /// <summary>
    /// Represents the EXI format as a <see cref="ContentFormat"/>.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.ContentFormat" />
    public sealed class ExiFormat : ContentFormat
    {
        /// <summary>
        /// The identifier number of the <see cref="ExiFormat"/>.
        /// </summary>
        private const uint NUMBER = 47;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExiFormat"/> class.
        /// </summary>
        public ExiFormat()
            : base(NUMBER, CoapMediaTypeNames.Application.Exi)
        {
        }
    }
}
