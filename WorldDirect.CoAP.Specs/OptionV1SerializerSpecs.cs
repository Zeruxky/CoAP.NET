namespace WorldDirect.CoAP.Specs
{
    using System.Collections.Generic;
    using System.Text;
    using FluentAssertions;
    using Options;
    using Xunit;
    using Version = CoAP.Version;

    public class OptionV1SerializerSpecs
    {
        [Fact]
        public void CanSerializeOptionsFromV1Message()
        {
            OptionV1Serializer.CanSerialize(Version.V1).Should().BeTrue();
        }

        [Fact]
        public void CanSerializeIfMatchOption()
        {
            var expectedBytes = new byte[]
            {
                22, 54, 55, 97, 98, 52, 51
            };
            var ifMatch = new IfMatch(Encoding.UTF8.GetBytes("67ab43"));
            var bytes = OptionV1Serializer.Serialize(ifMatch);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeUriHostOption()
        {
            var expectedBytes = new byte[]
            {
                61, 38, 100, 101, 118, 101, 108, 111, 112, 101, 114, 46, 99, 100, 110, 46, 109, 111, 122, 105, 108, 108, 97, 46, 110, 101, 116
            };
            var uriHost = new UriHost("developer.cdn.mozilla.net");
            var bytes = OptionV1Serializer.Serialize(uriHost);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeETagOption()
        {
            var expectedBytes = new byte[] { 68, 48, 56, 49, 53 };
            var eTag = new ETag(Encoding.UTF8.GetBytes("0815"));
            var bytes = OptionV1Serializer.Serialize(eTag);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeIfNoneMatchOption()
        {
            var expectedBytes = new byte[] { 80 };
            var ifNoneMatch = new IfNoneMatch();
            var bytes = OptionV1Serializer.Serialize(ifNoneMatch);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeUriPortOption()
        {
            var expectedBytes = new byte[] { 116, 51, 22, 0, 0 };
            var uriPort = new UriPort(5683);
            var bytes = OptionV1Serializer.Serialize(uriPort);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeLocationPathOption()
        {
            var expectedBytes = new byte[] { 139, 47, 105, 110, 100, 101, 120, 46, 104, 116, 109, 108 };
            var locationPath = new LocationPath("/index.html");
            var bytes = OptionV1Serializer.Serialize(locationPath);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeUriPath()
        {
            var expectedBytes = new byte[] { 187, 116, 101, 109, 112, 101, 114, 97, 116, 117, 114, 101 };
            var uriPath = new UriPath("temperature");
            var bytes = OptionV1Serializer.Serialize(uriPath);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeTextPlainContentFormat()
        {
            var expectedBytes = new byte[] { 196, 0, 0, 0, 0 };
            var contentFormat = ContentFormat.TextPlain;
            var bytes = OptionV1Serializer.Serialize(contentFormat);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeLinkContentFormat()
        {
            var expectedBytes = new byte[] { 196, 40, 0, 0, 0 };
            var contentFormat = ContentFormat.LinkFormat;
            var bytes = OptionV1Serializer.Serialize(contentFormat);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeXmlContentFormat()
        {
            var expectedBytes = new byte[] { 196, 41, 0, 0, 0 };
            var contentFormat = ContentFormat.XmlFormat;
            var bytes = OptionV1Serializer.Serialize(contentFormat);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeOctetStreamContentFormat()
        {
            var expectedBytes = new byte[] { 196, 42, 0, 0, 0 };
            var contentFormat = ContentFormat.OctetStreamFormat;
            var bytes = OptionV1Serializer.Serialize(contentFormat);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeExiContentFormat()
        {
            var expectedBytes = new byte[] { 196, 47, 0, 0, 0 };
            var contentFormat = ContentFormat.ExiFormat;
            var bytes = OptionV1Serializer.Serialize(contentFormat);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeJsonContentFormat()
        {
            var expectedBytes = new byte[] { 196, 50, 0, 0, 0 };
            var contentFormat = ContentFormat.Json;
            var bytes = OptionV1Serializer.Serialize(contentFormat);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeMaxAgeFormat()
        {
            var expectedBytes = new byte[] { 212, 27, 30, 0, 0, 0 };
            var maxAge = new MaxAge(30);
            var bytes = OptionV1Serializer.Serialize(maxAge);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeUriQuery()
        {
            var expectedBytes = new byte[] { 210, 28, 47, 47 };
            var uriQuery = new UriQuery("//");
            var bytes = OptionV1Serializer.Serialize(uriQuery);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeAccept()
        {
            var expectedBytes = new byte[] { 212, 30, 30, 0, 0, 0 };
            var accept = new Accept(30);
            var bytes = OptionV1Serializer.Serialize(accept);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeLocationQuery()
        {
            var expectedBytes = new byte[] { 211, 33, 97, 98, 99 };
            var locationQuery = new LocationQuery("abc");
            var bytes = OptionV1Serializer.Serialize(locationQuery);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeProxyUri()
        {
            var expectedBytes = new byte[] { 213, 48, 112, 114, 111, 120, 121 };
            var proxyUri = new ProxyUri("proxy");
            var bytes = OptionV1Serializer.Serialize(proxyUri);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeProxyScheme()
        {
            var expectedBytes = new byte[] { 214, 52, 115, 99, 104, 101, 109, 101 };
            var proxyScheme = new ProxyScheme("scheme");
            var bytes = OptionV1Serializer.Serialize(proxyScheme);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeSize1()
        {
            var expectedBytes = new byte[] { 212, 73, 30, 0, 0, 0 };
            var size1 = new Size1(30);
            var bytes = OptionV1Serializer.Serialize(size1);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void CanSerializeTwoOptions()
        {
            var expectedBytes = new byte[]
            {
                22, 54, 55, 97, 98, 52, 51, 45, 38, 100, 101, 118, 101, 108, 111, 112, 101, 114, 46, 99, 100, 110, 46, 109, 111, 122, 105, 108, 108, 97, 46,
                110, 101, 116
            };
            var ifMatch = new IfMatch(Encoding.UTF8.GetBytes("67ab43"));
            var uriHost = new UriHost("developer.cdn.mozilla.net");
            var options = new List<IOption>() {ifMatch, uriHost,};
            var bytes = OptionV1Serializer.Serialize(options);
            bytes.Should().BeEquivalentTo(expectedBytes);
        }
    }
}
