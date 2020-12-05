namespace WorldDirect.CoAP.Specs
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class UInt3Specs
    {
        private readonly UInt3 cut;

        public UInt3Specs()
        {
            this.cut = new UInt3();
        }

        [Fact]
        public void DefaultTypeValueIsZero()
        {
            this.cut.Should().Be((UInt3)0);
        }

        [Fact]
        public void TheMaximumValueIsSeven()
        {
            UInt3.MaxValue.Should().Be((UInt3)7);
        }

        [Fact]
        public void TheMinimumValueIsZero()
        {
            UInt3.MinValue.Should().Be((UInt3)0);
        }

        [Fact]
        public void UInt3CanNotBeGreaterThanSeven()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => (UInt3)8);
        }
    }
}
