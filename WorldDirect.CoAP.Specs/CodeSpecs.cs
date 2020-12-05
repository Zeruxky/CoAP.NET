namespace WorldDirect.CoAP.Specs
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    public class CodeSpecs
    {
        [Fact]
        public void CodeCanParseClassValue()
        {
            const byte content = 0x42;
            var actual = new Code(content);
            ((UInt3)actual.Class).Should().Be((UInt3)2);
        }

        [Fact]
        public void CodeCanParseDetailValue()
        {
            const byte content = 0x42;
            var actual = new Code(content);
            ((UInt5)actual.Detail).Should().Be((UInt5)2);
        }

        [Fact]
        public void CodeCanBeConstructedWithClassAndDetail()
        {
            var @class = (CodeClass)(UInt3)2;
            var detail = (CodeDetail)(UInt5)4;
            var cut = new Code(@class, detail);
            cut.Class.Should().Be(@class);
            cut.Detail.Should().Be(detail);
        }
    }

    public class MessageIdSpecs
    {
        [Fact]
        public void DefaultTypeValueIsZero()
        {
            var messageId = MessageId.Default;
            messageId.Should().Be((MessageId)0);
        }

        [Fact]
        public async Task MultipleThreadsCanGetANewMessageId()
        {
            var tasks = Enumerable.Range(0, 10).Select(t => MessageId.NewMessageIdAsync());
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
