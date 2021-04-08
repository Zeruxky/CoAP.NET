namespace WorldDirect.CoAP.Specs
{
    using System;
    using Common.Extensions;
    using FluentAssertions;
    using V1.Options;
    using Xunit;

    public class IfMatchFactorySpecs
    {
        private readonly IfMatch.Factory cut;

        public IfMatchFactorySpecs()
        {
            this.cut = new IfMatch.Factory();
        }

        [Fact]
        public void FactoryCanCreateIfMatch()
        {
            const string hexString = "be bf 87 3b 16 8c 00 06";
            var bytes = ByteArrayExtensions.FromHexString(hexString);

            var expectedIfMatch = new IfMatch(bytes);
            var optionData = new OptionData(0, 1, (ushort)bytes.Length, bytes);
            var actualIfMatch = this.cut.Create(optionData);

            actualIfMatch.Should().Be(expectedIfMatch);
        }

        [Fact]
        public void FactoryOnlyAcceptsOptionDataNumberOne()
        {
            const string hexString = "be bf 87 3b 16 8c 00 06";
            var bytes = ByteArrayExtensions.FromHexString(hexString);
            Assert.Throws<ArgumentException>(() => this.cut.Create(new OptionData(0, 2, (ushort)bytes.Length, bytes)));
        }

        [Fact]
        public void FactoryNumberIsIfMatchNumber()
        {
            this.cut.Number.Should().Be(IfMatch.NUMBER);
        }
    }
}
