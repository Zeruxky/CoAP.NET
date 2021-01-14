namespace WorldDirect.CoAP.Messages.Codes
{
    using System.Collections.Generic;

    public class CodeRegistry
    {
        public CodeRegistry()
        {
            this.Codes = new List<CoapCode>()
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

        public CodeRegistry(IEnumerable<CoapCode> codes)
        {
            this.Codes = new List<CoapCode>(codes);
        }

        public IList<CoapCode> Codes { get; }
    }
}
