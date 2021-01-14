namespace WorldDirect.CoAP.Messages.Codes
{
    public class RequestCode : CoapCode
    {
        public RequestCode(CodeDetail detail)
            : base(new CodeClass((UInt3)0), detail)
        {
        }
    }
}
