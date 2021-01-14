﻿// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using WorldDirect.CoAP.Messages;
    using WorldDirect.CoAP.Messages.Codes;
    using WorldDirect.CoAP.Messages.Options;
    using Xunit;

    /// <summary>
    /// Provides all specs that are related to <see cref="CoapMessageV1Deserializer"/>.
    /// </summary>
    public class CoapMessageV1DeserializerSpecs
    {
        private readonly CoapMessageV1Deserializer cut;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapMessageV1DeserializerSpecs"/> class.
        /// </summary>
        public CoapMessageV1DeserializerSpecs()
        {
            this.cut = new CoapMessageV1Deserializer(new CodeRegistry(), new OptionsFactory());
        }

        /// <summary>
        /// The deserializer can deserialize a message with payload.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous test method.</returns>
        [Fact]
        public async Task TheDeserializerCanDeserializeAMessageWithPayload()
        {
            var bytes = new byte[]
            {
                0x54, 0x45, 0xca, 0x3d, 0x8e, 0xef, 0x00, 0x00, 0x15, 0x56, 0x61, 0x6C, 0x75, 0x65, 0xFF, 0x33, 0x33, 0x2e, 0x38
            };

            var payload = new CoapPayload(new byte[] { 0x33, 0x33, 0x2e, 0x38 });
            var options = new List<ICoapOption>() { new IfMatch(Encoding.UTF8.GetBytes("Value"))};
            var header = new CoapHeader(CoapVersion.V1, CoapType.NonConfirmable, new CoapTokenLength((UInt4)4), SuccessfulResponseCode.Content, (CoapMessageId)51773);
            var expectedMessage = new CoapMessage(header, new CoapToken(0x0000ef8e, new CoapTokenLength((UInt4)4)), options, payload);

            await this.cut.InsertAsync(bytes, CancellationToken.None).ConfigureAwait(false);
            var message = await this.cut.GetAsync(CancellationToken.None).ConfigureAwait(false);
            message.Should().Be(expectedMessage);
        }

        /// <summary>
        /// The deserializer can deserialize a message without payload.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous test method.</returns>
        [Fact]
        public async Task DeserializeMessageWithoutPayload()
        {
            var bytes = new byte[]
            {
                0x54, 0x45, 0xca, 0x3d, 0x8e, 0xef, 0x00, 0x00, 0x15, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x05, 0x56, 0x61, 0x6C, 0x75, 0x65,
            };

            var options = new List<ICoapOption>() { new IfMatch(Encoding.UTF8.GetBytes("Value")), new IfMatch(Encoding.UTF8.GetBytes("Value")), };
            var header = new CoapHeader(CoapVersion.V1, CoapType.NonConfirmable, new CoapTokenLength((UInt4)4), SuccessfulResponseCode.Content, (CoapMessageId)51773);
            var expectedMessage = new CoapMessage(header, new CoapToken(0x0000ef8e, new CoapTokenLength((UInt4)4)), options, CoapPayload.EmptyPayload);

            await this.cut.InsertAsync(bytes, CancellationToken.None).ConfigureAwait(false);
            var message = await this.cut.GetAsync(CancellationToken.None).ConfigureAwait(false);
            message.Should().Be(expectedMessage);
        }

        /// <summary>
        /// The deserializer throws a <see cref="MessageFormatErrorException"/> if a payload marker without a payload is given.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous test method.</returns>
        [Fact]
        public async Task DeserializerThrowsMessageFormatErrorExceptionIfPayloadMarkerWithoutAPayloadIsGiven()
        {
            var bytes = new byte[]
            {
                0x54, 0x45, 0xca, 0x3d, 0x8e, 0xef, 0x00, 0x00, 0x15, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x05, 0x56, 0x61, 0x6C, 0x75, 0x65, 0xFF,
            };

            await this.cut.InsertAsync(bytes, CancellationToken.None).ConfigureAwait(false);
            await Assert.ThrowsAsync<MessageFormatErrorException>(() => this.cut.GetAsync(CancellationToken.None))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// The deserializer can get a <see cref="CoapHeader"/>.
        /// </summary>
        [Fact]
        public void DeserializerCanGetCoapHeader()
        {
            var bytes = new byte[] { 0x54, 0x45, 0xca, 0x3d, };
            var expectedHeader = new CoapHeader(CoapVersion.V1, CoapType.NonConfirmable, new CoapTokenLength((UInt4)4), SuccessfulResponseCode.Content, (CoapMessageId)51773);

            var header = this.cut.GetHeader(bytes);
            header.Should().Be(expectedHeader);
        }

        /// <summary>
        /// The deserializer can get a <see cref="CoapToken"/>.
        /// </summary>
        [Fact]
        public void DeserializerCanGetCoapToken()
        {
            var bytes = new byte[] { 0x8e, 0xef, 0x00, 0x00 };
            var tokenLength = new CoapTokenLength((UInt4)4);
            var expectedToken = new CoapToken(0x0000ef8e, tokenLength);

            var token = this.cut.GetToken(bytes, tokenLength);
            token.Should().Be(expectedToken);
        }

        /// <summary>
        /// The deserializer can get a single <see cref="ICoapOption"/>.
        /// </summary>
        [Fact]
        public void DeserializerCanGetSingleOption()
        {
            var bytes = new byte[] {0x15, 0x56, 0x61, 0x6C, 0x75, 0x65, };
            var expectedOptions = new List<ICoapOption>() { new IfMatch(Encoding.UTF8.GetBytes("Value")) };

            var options = this.cut.GetOptions(bytes);
            options.Should().BeEquivalentTo(expectedOptions);
        }

        /// <summary>
        /// The deserializer can get multiple <see cref="ICoapOption"/>s.
        /// </summary>
        [Fact]
        public void DeserializerCanGetMultipleOptions()
        {
            var bytes = new byte[] { 0x15, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x05, 0x56, 0x61, 0x6C, 0x75, 0x65 };
            var expectedOptions = new List<ICoapOption>() { new IfMatch(Encoding.UTF8.GetBytes("Value")), new IfMatch(Encoding.UTF8.GetBytes("Value")) };

            var options = this.cut.GetOptions(bytes);
            options.Should().BeEquivalentTo(expectedOptions);
        }

        /// <summary>
        /// The deserializer throws a <see cref="MessageFormatErrorException"/> if only the option delta is 15 (0xFF).
        /// </summary>
        [Fact]
        public void DeserializerThrowsMessageFormatErrorExceptionIfOptionDeltaIs15()
        {
            var bytes = new byte[] { 0xF5, 0x56, 0x61, 0x6C, 0x75, 0x65, };

            Assert.Throws<MessageFormatErrorException>(() => this.cut.GetOptions(bytes));
        }

        /// <summary>
        /// The deserializer throws <see cref="MessageFormatErrorException"/> if only the option length is 15 (0xFF).
        /// </summary>
        [Fact]
        public void DeserializerThrowsMessageFormatErrorExceptionIfOptionLengthIs15()
        {
            var bytes = new byte[] { 0x0F, 0x56, 0x61, 0x6C, 0x75, 0x65, };

            Assert.Throws<MessageFormatErrorException>(() => this.cut.GetOptions(bytes));
        }

        /// <summary>
        /// The deserializer throws <see cref="InvalidOperationException"/> if the version is not 1.
        /// </summary>
        [Fact]
        public void DeserializerThrowsInvalidOperationExceptionIfVersionIsNot1()
        {
            var bytes = new byte[] { 0xD4, 0x45, 0xca, 0x3d };

            Assert.Throws<InvalidOperationException>(() => this.cut.GetHeader(bytes));
        }

        /// <summary>
        /// The deserializer throws <see cref="MessageFormatErrorException"/> if token length is over 8.
        /// </summary>
        [Fact]
        public void DeserializerThrowsMessageFormatErrorExceptionIfTokenLengthIsOver8()
        {
            var bytes = new byte[] { 0x59, 0x45, 0xca, 0x3d };

            Assert.Throws<MessageFormatErrorException>(() => this.cut.GetHeader(bytes));
        }
    }
}
