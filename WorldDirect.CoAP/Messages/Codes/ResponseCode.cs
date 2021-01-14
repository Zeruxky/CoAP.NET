namespace WorldDirect.CoAP.Messages.Codes
{
    using System;

    public class ResponseCode : CoapCode
    {
        public ResponseCode(CodeClass @class, CodeDetail detail)
            : base(@class, detail)
        {
        }
    }
}
