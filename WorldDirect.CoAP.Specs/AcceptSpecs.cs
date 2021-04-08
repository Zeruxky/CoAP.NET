namespace WorldDirect.CoAP.Specs
{
    using System;
    using FluentAssertions;
    using V1.Options;
    using Xunit;

    public class AcceptSpecs
    {
        [Fact]
        public void AcceptOnlyAcceptsTwoBytes()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Accept(ushort.MaxValue + 1));
        }

        [Fact]
        public void AcceptNumberIs17()
        {
            Accept.NUMBER.Should().Be(17);
        }
    }
}