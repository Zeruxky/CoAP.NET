namespace WorldDirect.CoAP.Specs
{
    using FluentAssertions;
    using Xunit;

    public class TypeSpecs
    {
        [Fact]
        public void ConfirmableTypeIsBinaryValue00()
        {
            var confirmable = Type.Confirmable;
            ((UInt2)confirmable).Should().Be((UInt2)0);
        }

        [Fact]
        public void NonConfirmableTypeIsBinaryValue01()
        {
            var nonConfirmable = Type.NonConfirmable;
            ((UInt2)nonConfirmable).Should().Be((UInt2)1);
        }

        [Fact]
        public void AcknowledgementTypeIsBinaryValue10()
        {
            var acknowledgement = Type.Acknowledgement;
            ((UInt2)acknowledgement).Should().Be((UInt2)2);
        }

        [Fact]
        public void RestTypeIsBinaryValue11()
        {
            var reset = Type.Reset;
            ((UInt2)reset).Should().Be((UInt2)3);
        }
    }
}