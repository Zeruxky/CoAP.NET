// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Specs
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using WorldDirect.CoAP.V1.Messages;
    using Xunit;

    /// <summary>
    /// Provides all specs for <see cref="CoapMessageId"/>.
    /// </summary>
    public class CoapMessageIdSpecs
    {
        /// <summary>
        /// The default value of <see cref="CoapMessage"/> is 0.
        /// </summary>
        [Fact]
        public void DefaultTypeValueIsZero()
        {
            var messageId = CoapMessageId.Default;
            messageId.Should().Be((CoapMessageId)0);
        }
    }
}
