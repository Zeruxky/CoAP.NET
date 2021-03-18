namespace WorldDirect.CoAP.Specs
{
    using System;
    using FluentAssertions;
    using V1.Options;
    using Xunit;

    public class UriPortSpecs
    {
        [Fact]
        public void UriPortOnlyAcceptsTwoBytes()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new UriPort(ushort.MaxValue + 1));
        }

        [Fact]
        public void UriPortNumberIsSeven()
        {
            UriPort.NUMBER.Should().Be(7);
        }
    }
}