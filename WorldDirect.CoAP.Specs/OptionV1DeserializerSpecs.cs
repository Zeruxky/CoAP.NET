namespace WorldDirect.CoAP.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Common.Extensions;
    using FakeItEasy;
    using FluentAssertions;
    using Options;
    using Xunit;
    using Version = CoAP.Version;

    public class OptionV1DeserializerSpecs
    {
        private readonly OptionsRegistry registry;

        public OptionV1DeserializerSpecs()
        {
            var registryValues = new Dictionary<ushort, Func<byte[], IOption>>()
            {
                {1, value => new IfMatch(value)},
                {3, value => new UriHost(Encoding.UTF8.GetString(value))},
                {4, value => new ETag(value)},
                {5, value => new IfNoneMatch()},
                {7, value => new UriPort(BitConverter.ToUInt16(value, 0))},
                {8, value => new LocationPath(Encoding.UTF8.GetString(value))},
                {11, value => new UriPath(Encoding.UTF8.GetString(value))},
                {12, value => new ContentFormat(BitConverter.ToUInt16(value, 0))},
                {14, value => new MaxAge(BitConverter.ToUInt16(value))},
                {15, value => new UriQuery(Encoding.UTF8.GetString(value))},
                {17, value => new Accept(BitConverter.ToUInt16(value))},
                {20, value => new LocationQuery(Encoding.UTF8.GetString(value))},
                {35, value => new ProxyUri(Encoding.UTF8.GetString(value))},
                {39, value => new ProxyScheme(Encoding.UTF8.GetString(value))},
                {60, value => new Size1(BitConverter.ToUInt16(value))},
            };

            this.registry = new OptionsRegistry();
            foreach (var (number, factory) in registryValues)
            {
                this.registry.Add(number, factory);   
            }
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
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(3));
        }

        [Fact]
        public void CanDeserializeETag()
        {
            var value = new byte[] { 68, 48, 56, 49, 53, 0xFF };
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(4));
        }

        [Fact]
        public void CanDeserializeIfNoneMatch()
        {
            var value = new byte[] { 80, 0xFF };
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(5));
        }

        [Fact]
        public void CanDeserializeUriPort()
        {
            var value = new byte[] { 116, 51, 22, 0, 0, 0xFF };
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(7));
        }

        [Fact]
        public void CanDeserializeLocationPath()
        {
            var value = new byte[] { 139, 47, 105, 110, 100, 101, 120, 46, 104, 116, 109, 108, 0xFF };
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(8));
        }

        [Fact]
        public void CanDeserializeUriPath()
        {
            var value = new byte[] { 187, 116, 101, 109, 112, 101, 114, 97, 116, 117, 114, 101, 0xFF };
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(11));
        }

        [Fact]
        public void CanDeserializeContentFormat()
        {
            var value = new byte[] { 196, 50, 0, 0, 0, 0xFF };
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(12));
        }

        [Fact]
        public void CanDeserializeMaxAge()
        {
            var value = new byte[] { 13, 27, 30, 0, 0, 0, 0xFF };
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(14));
        }

        [Fact]
        public void CanDeserializeUriQuery()
        {
            var value = new byte[] { 210, 28, 47, 47, 0xFF };
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(15));
        }

        [Fact]
        public void CanDeserializerAccept()
        {
            var value = new byte[] {212, 30, 30, 0, 0, 0, 0xFF};
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(17));
        }

        [Fact]
        public void CanDeserializeLocationQuery()
        {
            var value = new byte[] {211, 33, 97, 98, 99, 0xFF};
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(20));
        }

        [Fact]
        public void CanDeserializeProxyUri()
        {
            var value = new byte[] {213, 48, 112, 114, 111, 120, 121, 0xFF};
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(35));
        }

        [Fact]
        public void CanDeserializeProxyScheme()
        {
            var value = new byte[] {214, 52, 115, 99, 104, 101, 109, 101, 0xFF};
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(39));
        }

        [Fact]
        public void CanDeserializerSize1()
        {
            var value = new byte[] {212, 73, 30, 0, 0, 0, 0xFF};
            var options = OptionV1Deserializer.Deserialize(value, this.registry);
            options.Should().ContainSingle(o => o.Number.Equals(60));
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
