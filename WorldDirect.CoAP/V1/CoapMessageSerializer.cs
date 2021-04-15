// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1
{
    using System;
    using System.Buffers;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO.Pipelines;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Codes;
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

    public class MessageHandler : ICoapMessageHandler
    {
        private readonly RequestHandler requestHandler;
        private readonly ResponseHandler responseHandler;

        /// <inheritdoc />
        public bool CanHandle(CoapMessageContext ctx)
        {
            return ctx.Message is CoapMessage;
        }

        /// <inheritdoc />
        public async Task HandleAsync(CoapMessageContext ctx, CancellationToken ct)
        {
            var msg = (CoapMessage)ctx.Message;
            var optionCollection = new OptionCollection(msg.Options);
            if (msg.Header.Code is RequestCode)
            {
                this.requestHandler.HandleAsync(msg, ct)
            }

            if (msg.Header.Code is ResponseCode)
            {
                this.responseHandler.HandleAsync(msg, ct);
            }

            throw new ArgumentException();
        }
    }

    public class RequestHandler
    {

    }

    public interface IEndpoint
    {
        public int Port { get; }

        public string Address { get; }
    }

    public class UdpEndpoint : IEndpoint
    {
        private readonly IPEndPoint endPoint;

        public UdpEndpoint(IPEndPoint endpoint)
        {
            this.endPoint = endpoint;
        }

        public int Port => this.endPoint.Port;

        public string Address => this.endPoint.Address.ToString();

        public override string ToString() => this.endPoint.ToString();
    }

    public class CoapMessageContext
    {
        public CoapMessageContext(CoapConnection connection, ICoapMessage message)
        {
            Connection = connection;
            Message = message;
        }

        public CoapConnection Connection { get; }

        public ICoapMessage Message { get; }
    }

    public interface IChannel
    {
        IEndpoint LocalEndpoint { get; }

        IEndpoint RemoteEndpoint { get; }

        Task SendAsync(CoapMessage message, CancellationToken ct);

        IAsyncEnumerable<CoapMessage> ReceiveAsync(CancellationToken ct);
    }

    public class UdpChannel : IChannel
    {
        public UdpChannel(IPEndPoint localEndpoint, IPEndPoint remoteEndPoint)
        {
            this.LocalEndpoint = new UdpEndpoint(localEndpoint);
            this.RemoteEndpoint = new UdpEndpoint(remoteEndPoint);
        }

        public IEndpoint LocalEndpoint { get; }

        public IEndpoint RemoteEndpoint { get; }

        public async Task SendAsync(CoapMessage message, CancellationToken ct)
        {

        }

        public async IAsyncEnumerable<CoapMessage> ReceiveAsync(CancellationToken ct)
        {

        }
    }
}
