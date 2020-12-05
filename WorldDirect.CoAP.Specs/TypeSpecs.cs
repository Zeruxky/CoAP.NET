namespace WorldDirect.CoAP.Specs
{
    using FluentAssertions;
    using Xunit;

    public class TypeSpecs
    {
        [Fact]
        public void ConfirmableTypeIsValueZero()
        {
            var confirmable = Type.Confirmable;
            ((UInt2)confirmable).Should().Be((UInt2)0);
        }

        [Fact]
        public void NonConfirmableTypeIsValueOne()
        {
            var nonConfirmable = Type.NonConfirmable;
            ((UInt2)nonConfirmable).Should().Be((UInt2)1);
        }

        [Fact]
        public void AcknowledgementTypeIsValueTwo()
        {
            var acknowledgement = Type.Acknowledgement;
            ((UInt2)acknowledgement).Should().Be((UInt2)2);
        }

        [Fact]
        public void RestTypeIsValueThree()
        {
            var reset = Type.Reset;
            ((UInt2)reset).Should().Be((UInt2)3);
        }
    }
}
