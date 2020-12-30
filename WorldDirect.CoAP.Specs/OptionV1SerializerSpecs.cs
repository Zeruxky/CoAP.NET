namespace WorldDirect.CoAP.Specs
{
    using System.Text;
    using FluentAssertions;
    using Options;
    using Xunit;

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
    }
}