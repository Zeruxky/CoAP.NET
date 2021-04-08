namespace WorldDirect.CoAP.Specs
{
    using System;
    using System.Buffers.Binary;
    using FluentAssertions;
    using V1.Options;
    using Xunit;

    public class AcceptFactorySpecs
    {
        private readonly Accept.Factory cut;

        public AcceptFactorySpecs()
        {
            this.cut = new Accept.Factory();
        }

        [Fact]
        public void FactoryCanCreateAccept()
        {
            var content = new byte[4];
            var expectedAccept = new Accept(1);
            BinaryPrimitives.WriteUInt32BigEndian(content, 1);

            var optionData = new OptionData(0, Accept.NUMBER, (ushort)content.Length, content);
            var actualAccept = this.cut.Create(optionData);

            actualAccept.Should().Be(expectedAccept);
        }

        [Fact]
        public void FactoryOnlyAcceptsOptionDataWithAcceptNumber()
        {
            var content = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(content, 1);

            Assert.Throws<ArgumentException>(() => this.cut.Create(new OptionData(0, 0, (ushort)content.Length, content)));
        }
    }
}