namespace WorldDirect.CoAP.Messages.Codes
{
    public class ServerResponseCode : ResponseCode
    {
        public static readonly ServerResponseCode InternalServerError = new ServerResponseCode(new CodeDetail((UInt5)0));
        public static readonly ServerResponseCode NotImplemented = new ServerResponseCode(new CodeDetail((UInt5)1));
        public static readonly ServerResponseCode BadGateway = new ServerResponseCode(new CodeDetail((UInt5)2));
        public static readonly ServerResponseCode ServiceUnavailable = new ServerResponseCode(new CodeDetail((UInt5)3));
        public static readonly ServerResponseCode GatewayTimeout = new ServerResponseCode(new CodeDetail((UInt5)4));
        public static readonly ServerResponseCode ProxyingNotSupported = new ServerResponseCode(new CodeDetail((UInt5)5));

        public ServerResponseCode(CodeDetail detail)
            : base(new CodeClass((UInt3)5), detail)
        {
        }
    }
}
