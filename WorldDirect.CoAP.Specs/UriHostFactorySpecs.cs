namespace WorldDirect.CoAP.Specs
{
    using System;
    using System.Text;
    using FluentAssertions;
    using V1.Options;
    using Xunit;

    public class UriHostFactorySpecs
    {
        private readonly UriHostFactory cut;

        public UriHostFactorySpecs()
        {
            this.cut = new UriHostFactory();
        }

        [Fact]
        public void FactoryCanCreateUriHost()
        {
            var expectedUriHost = new UriHost("example.net");
            var content = Encoding.UTF8.GetBytes("example.net");

            var optionData = new OptionData(0, 3, (ushort)content.Length, content);
            var actualUriHost = this.cut.Create(optionData);

            actualUriHost.Should().Be(expectedUriHost);
        }

        [Fact]
        public void FactoryOnlyAcceptsOptionDataNumberThree()
        {
            var content = Encoding.UTF8.GetBytes("example.net");
            Assert.Throws<ArgumentException>(() => this.cut.Create(new OptionData(0, 1, (ushort)content.Length, content)));
        }

        [Fact]
        public void FactoryNumberIsUriHostNumber()
        {
            this.cut.Number.Should().Be(UriHost.NUMBER);
        }
    }
}