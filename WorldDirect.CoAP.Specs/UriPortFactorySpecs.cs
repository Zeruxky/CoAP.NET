namespace WorldDirect.CoAP.Specs
{
    using System;
    using System.Buffers.Binary;
    using FluentAssertions;
    using V1.Options;
    using Xunit;

    public class UriPortFactorySpecs
    {
        private readonly UriPort.Factory cut;

        public UriPortFactorySpecs()
        {
            this.cut = new UriPort.Factory();
        }

        [Fact]
        public void FactoryCanCreateUriPort()
        {
            var content = new byte[4];
            var expectedUriPort = new UriPort(5683);
            BinaryPrimitives.WriteUInt32BigEndian(content, 5683);

            var optionData = new OptionData(0, UriPort.NUMBER, (ushort)content.Length, content);
            var actualUriPort = this.cut.Create(optionData);

            actualUriPort.Should().Be(expectedUriPort);
        }

        [Fact]
        public void FactoryOnlyAcceptsOptionDataWithUriPortNumber()
        {
            var content = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(content, 5683);

            Assert.Throws<ArgumentException>(() => this.cut.Create(new OptionData(0, 0, (ushort)content.Length, content)));
        }

        [Fact]
        public void FactoryNumberIsUriPortNumber()
        {
            this.cut.Number.Should().Be(UriPort.NUMBER);
        }
    }
}
