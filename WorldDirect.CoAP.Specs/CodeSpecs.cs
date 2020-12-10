namespace WorldDirect.CoAP.Specs
{
    using System.Net;
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

        [Fact]
        public void CodeCanBeConstructedWithClassAndDetail()
        {
            var @class = (CodeClass)(UInt3)2;
            var detail = (CodeDetail)(UInt5)4;
            var cut = new Code(@class, detail);
            cut.Class.Should().Be(@class);
            cut.Detail.Should().Be(detail);
        }
    }
}
