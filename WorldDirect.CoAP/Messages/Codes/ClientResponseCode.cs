namespace WorldDirect.CoAP.Messages.Codes
{
    public sealed class ClientResponseCode : ResponseCode
    {
        public static readonly ClientResponseCode BadRequest = new ClientResponseCode(new CodeDetail((UInt5)0));
        public static readonly ClientResponseCode Unauthorized = new ClientResponseCode(new CodeDetail((UInt5)1));
        public static readonly ClientResponseCode BadOption = new ClientResponseCode(new CodeDetail((UInt5)2));
        public static readonly ClientResponseCode Forbidden = new ClientResponseCode(new CodeDetail((UInt5)3));
        public static readonly ClientResponseCode NotFound = new ClientResponseCode(new CodeDetail((UInt5)4));
        public static readonly ClientResponseCode MethodNotAllowed = new ClientResponseCode(new CodeDetail((UInt5)5));
        public static readonly ClientResponseCode NotAcceptable = new ClientResponseCode(new CodeDetail((UInt5)6));
        public static readonly ClientResponseCode PreconditionFailed = new ClientResponseCode(new CodeDetail((UInt5)12));
        public static readonly ClientResponseCode RequestEntityTooLarge = new ClientResponseCode(new CodeDetail((UInt5)13));
        public static readonly ClientResponseCode UnsupportedContentFormat = new ClientResponseCode(new CodeDetail((UInt5)15));

        public ClientResponseCode(CodeDetail detail)
            : base(new CodeClass((UInt3)4), detail)
        {
        }
    }
}
