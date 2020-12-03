namespace WorldDirect.CoAP.Specs
{
    using System;
    using Xunit;

    public class UInt3Specs
    {
        [Fact]
        public void UInt2CanNotBeGreaterThan7()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => (UInt2)8);
        }
    }
}