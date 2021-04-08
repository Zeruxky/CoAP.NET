// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1
{
    using System;
    using System.Net.Sockets;
    using Microsoft.Extensions.Logging;
    using WorldDirect.CoAP.Common;
    using WorldDirect.CoAP.V1.Messages;

    /// <summary>
    /// Provides functionality to serialize and deserialize a <see cref="CoapMessage"/> that is compliant to RFC 7252.
    /// </summary>
    public sealed class CoapMessageSerializer : IMessageSerializer
    {
        private readonly IReader<ReadOnlyOptionCollection> optionReader;
        private readonly IReader<ReadOnlyMemory<byte>> payloadReader;
        private readonly ILogger<CoapMessageSerializer> logger;
        private readonly IReader<CoapHeader> headerReader;
        private readonly IReader<CoapToken> tokenReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapMessageSerializer"/> class.
        /// </summary>
        /// <param name="headerReader">The header reader.</param>
        /// <param name="tokenReader">The token reader.</param>
        /// <param name="optionsReader">The options reader.</param>
        /// <param name="payloadReader">The payload reader.</param>
        /// <param name="logger">The logger.</param>
        public CoapMessageSerializer(
            IReader<CoapHeader> headerReader,
            IReader<CoapToken> tokenReader,
            IReader<ReadOnlyOptionCollection> optionsReader,
            IReader<ReadOnlyMemory<byte>> payloadReader,
            ILogger<CoapMessageSerializer> logger)
        {
            this.headerReader = headerReader;
            this.tokenReader = tokenReader;
            this.optionReader = optionsReader;
            this.payloadReader = payloadReader;
            this.logger = logger;
        }

        /// <summary>
        /// Deserializes the given <paramref name="value" /> to a <see cref="CoapMessage" />.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlySpan{T}" /> of <see cref="byte" />s for deserialization.</param>
        /// <returns>
        /// A <see cref="CoapMessage" /> that is equivalent to the given <paramref name="value" />.
        /// </returns>
        public CoapMessage Deserialize(ReadOnlyMemory<byte> value)
        {
            var position = this.headerReader.Read(value.Slice(0, 4), out var header);
            position += this.tokenReader.Read(value.Slice(position, (UInt4)header.Length), out var token);
            position += this.optionReader.Read(value.Slice(position), out var options);
            position += this.payloadReader.Read(value.Slice(position), out var payload);

            var message = new CoapMessage(header, token, options, payload);
            this.logger.SuccessfullyDeserializedMessage(message.Header.Id, message.Token, position);
            return message;
        }

        /// <summary>
        /// Determines whether this instance can deserialize the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        ///   <c>true</c> if this instance can deserialize the specified result; otherwise, <c>false</c>.
        /// </returns>
        public bool CanDeserialize(UdpReceiveResult result)
        {
            this.headerReader.Read(result.Buffer, out var header);
            return header.Version.Equals(CoapVersion.V1);
        }
    }
}
