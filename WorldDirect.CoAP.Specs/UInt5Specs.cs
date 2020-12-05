namespace WorldDirect.CoAP.Specs
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class UInt5Specs
    {
        private readonly UInt5 cut;

        public UInt5Specs()
        {
            this.cut = new UInt5();
        }

        [Fact]
        public void DefaultTypeValueIsZero()
        {
            this.cut.Should().Be((UInt5)0);
        }

        [Fact]
        public void TheMaximumValueIs31()
        {
            UInt5.MaxValue.Should().Be((UInt5)31);
        }

        [Fact]
        public void TheMinimumValueIsZero()
        {
            UInt5.MinValue.Should().Be((UInt5)0);
        }

        [Fact]
        public void UInt4CanNotBeGreaterThan15()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => (UInt5)32);
        }
    }
}