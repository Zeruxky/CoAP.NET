namespace WorldDirect.CoAP.Specs
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Extensions;
    using FakeItEasy;
    using FluentAssertions;
    using Options;
    using Xunit;

    public class OptionV1DeserializerSpecs
    {
        private readonly OptionsRegistry registry;

        public OptionV1DeserializerSpecs()
        {
            var x = A.CollectionOfFake<IOption>(2);
            var options = new List<IOption>()
            {
                new IfMatch(new byte[0]),
                new UriHost(" "),
            };
            this.registry = new OptionsRegistry(options);
        }

        [Fact]
        public void CanDeserializeOptionsFromV1Message()
        {
            OptionV1Deserializer.CanDeserialize(Version.V1).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeIfMatchOption()
        {
            var value = new byte[] { 22, 54, 55, 97, 98, 52, 51, 0xFF };
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(1));
        }

        [Fact]
        public void CanDeserializeUriHost()
        {
            var value = new byte[]
            {
                61, 38, 100, 101, 118, 101, 108, 111, 112, 101, 114, 46, 99, 100, 110, 46, 109, 111, 122, 105, 108, 108, 97, 46, 110, 101, 116, 0xFF
            };
            var option = OptionV1Deserializer.Deserialize(value, this.registry);
            option.Should().ContainSingle(o => o.Number.Equals(3));
        }

        [Fact]
        public void CanDeserializeTwoOptions()
        {
            var value = new byte[]
            {
                22, 54, 55, 97, 98, 52, 51, 45, 38, 100, 101, 118, 101, 108, 111, 112, 101, 114, 46, 99, 100, 110, 46, 109, 111, 122, 105, 108, 108, 97, 46,
                110, 101, 116, 0xFF
            };
            var options = OptionV1Deserializer.Deserialize(value, this.registry).ToList();
            options.Should().HaveCount(2);
        }

        [Fact]
        public void ThrowsExceptionIfLengthIs15()
        {
            var option = new UriHost("abcdefghijklmno");
            var bytes = OptionV1Serializer.Serialize(option).Append(0xFF);
            Assert.Throws<MessageFormatError>(() => OptionV1Deserializer.Deserialize(bytes, this.registry));
        }

        [Fact]
        public void CanHandleIfNoOptionsAreProvided()
        {
            OptionV1Deserializer.Deserialize(new byte[]{ 0xFF }, this.registry);
        }
    }
}