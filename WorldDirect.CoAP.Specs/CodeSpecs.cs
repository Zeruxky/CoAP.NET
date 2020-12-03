namespace WorldDirect.CoAP.Specs
{
    using FluentAssertions;
    using Xunit;

    public class CodeSpecs
    {
        [Fact]
        public void CodeCanParseClassValue()
        {
            const byte content = 0x42;
            var actual = new Code(content);
            ((UInt3)actual.Class).Should().Be((UInt3)2);
        }

        [Fact]
        public void CodeCanParseDetailValue()
        {
            const byte content = 0x42;
            var actual = new Code(content);
            ((UInt5)actual.Detail).Should().Be((UInt5)2);
        }
    }
}