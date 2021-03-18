namespace WorldDirect.CoAP.Specs
{
    using System;
    using System.Buffers.Binary;
    using FluentAssertions;
    using V1.Options;
    using Xunit;

    public class AcceptSpecs
    {
        [Fact]
        public void AcceptOnlyAcceptsTwoBytes()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Accept(ushort.MaxValue + 1));
        }

        [Fact]
        public void AcceptNumberIs17()
        {
            Accept.NUMBER.Should().Be(17);
        }
    }

    public class AcceptFactorySpecs
    {
        private readonly AcceptFactory cut;

        public AcceptFactorySpecs()
        {
            this.cut = new AcceptFactory();
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

            Assert.Throws<ArgumentException>(() => this.cut.Create(new OptionData(0, Accept.NUMBER, (ushort)content.Length, content)));
        }
    }

    public class UriPortFactorySpecs
    {
        private readonly UriPortFactory cut;

        public UriPortFactorySpecs()
        {
            this.cut = new UriPortFactory();
        }

        [Fact]
        public void FactoryCanCreateUriPort()
        {
            var content = new byte[4];
            var expectedUriPort = new UriPort(5683);
            BinaryPrimitives.WriteUInt32BigEndian(content, 5683);

            var optionData = new OptionData(0, UriPort.NUMBER, (ushort)content.Length, content);
            var actualUriPort = this.cut.Create(optionData);

            actualUriPort.Should().Be(expectedUriPort);
        }

        [Fact]
        public void FactoryOnlyAcceptsOptionDataWithUriPortNumber()
        {
            var content = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(content, 5683);

            Assert.Throws<ArgumentException>(() => this.cut.Create(new OptionData(0, 0, (ushort)content.Length, content)));
        }

        [Fact]
        public void FactoryNumberIsUriPortNumber()
        {
            this.cut.Number.Should().Be(UriPort.NUMBER);
        }
    }
}
