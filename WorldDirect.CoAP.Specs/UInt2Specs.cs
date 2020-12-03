namespace WorldDirect.CoAP.Specs
{
    using System;
    using Xunit;

    public class UInt2Specs
    {
        [Fact]
        public void UInt2CanNotBeGreaterThan3()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => (UInt2)4);
        }
    }
}