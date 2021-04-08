namespace WorldDirect.CoAP.Specs
{
    using System;
    using System.Text;
    using FluentAssertions;
    using V1.Options;
    using Xunit;

    public class UriPathFactorySpecs
    {
        private readonly UriPath.Factory cut;

        public UriPathFactorySpecs()
        {
            this.cut = new UriPath.Factory();
        }

        [Fact]
        public void FactoryCanCreateUriPath()
        {
            var expectedUriPath = new UriPath(".well-known");
            var content = Encoding.UTF8.GetBytes(".well-known");

            var optionData = new OptionData(0, UriPath.NUMBER, (ushort)content.Length, content);
            var actualUriPath = this.cut.Create(optionData);

            actualUriPath.Should().Be(expectedUriPath);
        }

        [Fact]
        public void FactoryNumberIsUriPathNumber()
        {
            this.cut.Number.Should().Be(UriPath.NUMBER);
        }

        [Fact]
        public void FactoryOnlyAcceptsOptionDataNumberThree()
        {
            var content = Encoding.UTF8.GetBytes(".well-known");
            Assert.Throws<ArgumentException>(() => this.cut.Create(new OptionData(0, 4, (ushort)content.Length, content)));
        }
    }
}
