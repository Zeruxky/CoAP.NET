namespace WorldDirect.CoAP.Specs
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class UInt2Specs
    {
        private readonly UInt2 cut;

        public UInt2Specs()
        {
            this.cut = new UInt2();
        }

        [Fact]
        public void TheTypeDefaultValueIsZero()
        {
            this.cut.Should().Be((UInt2)0);
        }

        [Fact]
        public void TheMaximumValueIsThree()
        {
            UInt2.MaxValue.Should().Be((UInt2)3);
        }

        [Fact]
        public void TheMinimumValueIsZero()
        {
            UInt2.MinValue.Should().Be((UInt2)0);
        }

        [Fact]
        public void UInt2CanNotBeGreaterThanThree()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => (UInt2)4);
        }
    }
}
