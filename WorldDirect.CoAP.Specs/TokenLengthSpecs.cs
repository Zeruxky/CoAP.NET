namespace WorldDirect.CoAP.Specs
{
    using Xunit;

    public class TokenLengthSpecs
    {
        [Fact]
        public void TokenLengthCanNotBeGreaterThan8()
        {
            Assert.Throws<MessageFormatError>(() => new TokenLength((UInt4)9));
        }
    }
}