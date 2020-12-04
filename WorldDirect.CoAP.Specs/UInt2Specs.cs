namespace WorldDirect.CoAP.Specs
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class UInt2Specs
    {
        private readonly UInt2 cut;

        [Fact]
        public void TheTypeDefaultValueIsZero()
        {
            this.cut.Should().Be((UInt2)0);
        }

        [Fact]
        public void TheMaximumValueIs3()
        {
            UInt2.MaxValue.Should().Be((UInt2)3);
        }

        [Fact]
        public void UInt2CanNotBeGreaterThan3()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => (UInt2)4);
        }
    }
}
