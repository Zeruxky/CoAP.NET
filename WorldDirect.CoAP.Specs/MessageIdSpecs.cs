namespace WorldDirect.CoAP.Specs
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

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