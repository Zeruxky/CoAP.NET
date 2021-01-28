namespace WorldDirect.CoAP.Messages.Codes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides functionality to add and remove <see cref="CoapCode"/>s from the <see cref="CodeRegistry"/>.
    /// This registry stores all registered <see cref="CoapCode"/> for the whole application.
    /// </summary>
    public class CodeRegistry
    {
        private readonly List<CoapCode> codes;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeRegistry"/> class.
        /// </summary>
        public CodeRegistry()
        {
            this.codes = new List<CoapCode>()
            {
                new EmptyCode(),
                MethodCode.Get,
                MethodCode.Post,
                MethodCode.Put,
                MethodCode.Delete,
                SuccessfulResponseCode.Created,
                SuccessfulResponseCode.Deleted,
                SuccessfulResponseCode.Valid,
                SuccessfulResponseCode.Changed,
                SuccessfulResponseCode.Content,
                ClientResponseCode.BadRequest,
                ClientResponseCode.Unauthorized,
                ClientResponseCode.BadOption,
                ClientResponseCode.Forbidden,
                ClientResponseCode.NotFound,
                ClientResponseCode.MethodNotAllowed,
                ClientResponseCode.NotAcceptable,
                ClientResponseCode.PreconditionFailed,
                ClientResponseCode.RequestEntityTooLarge,
                ClientResponseCode.UnsupportedContentFormat,
                ServerResponseCode.InternalServerError,
                ServerResponseCode.NotImplemented,
                ServerResponseCode.BadGateway,
                ServerResponseCode.ServiceUnavailable,
                ServerResponseCode.GatewayTimeout,
                ServerResponseCode.ProxyingNotSupported,
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeRegistry"/> class.
        /// </summary>
        /// <param name="codes">The set of <see cref="CoapCode"/>s that should represent the new created <see cref="CodeRegistry"/>.</param>
        public CodeRegistry(IEnumerable<CoapCode> codes)
        {
            this.codes = new List<CoapCode>(codes);
        }

        /// <summary>
        /// Adds the given <see cref="CoapCode"/> to the <see cref="CodeRegistry"/>.
        /// </summary>
        /// <param name="code">The <see cref="CoapCode"/> that should be added to the <see cref="CodeRegistry"/>.</param>
        /// <remarks>If the given <paramref name="code"/> already exists in the <see cref="CodeRegistry"/> then the method
        /// returns immediately.</remarks>
        public void Add(CoapCode code)
        {
            if (this.Exists(code))
            {
                return;
            }

            this.codes.Add(code);
        }

        /// <summary>
        /// Gets the <see cref="CoapCode"/> with the given <paramref name="class"/> and <paramref name="detail"/>.
        /// </summary>
        /// <param name="class">The <see cref="CodeClass"/> of the required <see cref="CoapCode"/>.</param>
        /// <param name="detail">The <see cref="CodeDetail"/> of the required <see cref="CoapCode"/>.</param>
        /// <returns>The <see cref="CoapCode"/> that matches the given <paramref name="class"/> and <see cref="detail"/>.</returns>
        /// <exception cref="ArgumentException">Throws if none <see cref="CoapCode"/> exists in the <see cref="CodeRegistry"/> with the given <paramref name="class"/> and <paramref name="detail"/>.</exception>
        public CoapCode Get(CodeClass @class, CodeDetail detail)
        {
            if (!this.Exists(@class, detail))
            {
                throw new ArgumentException($"Unknown code {@class}.{detail}.");
            }

            var code = this.codes.Single(c => c.Class.Equals(@class) && c.Detail.Equals(detail));
            return code;
        }

        private bool Exists(CoapCode code)
        {
            return this.codes.Any(c => c.Equals(code));
        }

        private bool Exists(CodeClass @class, CodeDetail detail)
        {
            return this.codes.Any(c => c.Class.Equals(@class) && c.Detail.Equals(detail));
        }
    }
}
