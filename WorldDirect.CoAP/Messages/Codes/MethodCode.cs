namespace WorldDirect.CoAP.Messages.Codes
{
    public sealed class MethodCode : RequestCode
    {
        public static readonly MethodCode Get = new MethodCode(new CodeDetail((UInt5)1));
        public static readonly MethodCode Post = new MethodCode(new CodeDetail((UInt5)2));
        public static readonly MethodCode Put = new MethodCode(new CodeDetail((UInt5)3));
        public static readonly MethodCode Delete = new MethodCode(new CodeDetail((UInt5)4));

        public MethodCode(CodeDetail detail)
            : base(detail)
        {
        }
    }
}
