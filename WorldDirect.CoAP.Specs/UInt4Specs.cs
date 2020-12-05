namespace WorldDirect.CoAP.Specs
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class UInt4Specs
    {
        private readonly UInt4 cut;

        public UInt4Specs()
        {
            this.cut = new UInt4();
        }

        [Fact]
        public void DefaultTypeValueIsZero()
        {
            this.cut.Should().Be((UInt4)0);
        }

        [Fact]
        public void TheMaximumValueIs15()
        {
            UInt4.MaxValue.Should().Be((UInt4)15);
        }

        [Fact]
        public void TheMinimumValueIsZero()
        {
            UInt4.MinValue.Should().Be((UInt4)0);
        }

        [Fact]
        public void UInt4CanNotBeGreaterThan15()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => (UInt4)16);
        }
    }
}