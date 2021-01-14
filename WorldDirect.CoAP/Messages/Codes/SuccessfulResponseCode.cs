namespace WorldDirect.CoAP.Messages.Codes
{
    public sealed class SuccessfulResponseCode : ResponseCode
    {
        public static readonly SuccessfulResponseCode Created = new SuccessfulResponseCode(new CodeDetail((UInt5)1));
        public static readonly SuccessfulResponseCode Deleted = new SuccessfulResponseCode(new CodeDetail((UInt5)2));
        public static readonly SuccessfulResponseCode Valid = new SuccessfulResponseCode(new CodeDetail((UInt5)3));
        public static readonly SuccessfulResponseCode Changed = new SuccessfulResponseCode(new CodeDetail((UInt5)4));
        public static readonly SuccessfulResponseCode Content = new SuccessfulResponseCode(new CodeDetail((UInt5)5));

        public SuccessfulResponseCode(CodeDetail detail)
            : base(new CodeClass((UInt3)2), detail)
        {
        }
    }
}
