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
    using System.Threading.Tasks.Dataflow;
    using Codes;
    using Codes.MethodCodes;
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
        public bool CanDeserialize(ReadOnlyMemory<byte> result)
        {
            this.headerReader.Read(result, out var header);
            return this.CanDeserialize(header.Version);
        }

        public bool CanDeserialize(CoapVersion version)
        {
            return version.Equals(CoapVersion.V1);
        }
    }

    //public class MessageHandler : ICoapMessageHandler
    //{
    //    private readonly RequestHandler requestHandler;
    //    private readonly ResponseHandler responseHandler;

    //    /// <inheritdoc />
    //    public bool CanHandle(CoapMessageContext ctx)
    //    {
    //        return ctx.Message is CoapMessage;
    //    }

    //    /// <inheritdoc />
    //    public async Task HandleAsync(RawCoapMessage msg, CancellationToken ct)
    //    {
    //        if (msg.Header.Code is RequestCode)
    //        {
    //            this.requestHandler.HandleAsync(msg, ct);
    //        }

    //        if (msg.Header.Code is ResponseCode)
    //        {
    //            this.responseHandler.HandleAsync(msg, ct);
    //        }

    //        throw new ArgumentException();
    //    }
    //}

    public class ResponseHandler : IResponseHandler, ITargetBlock<CoapMessage>, IReceivableSourceBlock<CoapMessageContext>
    {
        public bool CanHandle(CoapVersion version)
        {
            throw new NotImplementedException();
        }

        public Task HandleAsync(CoapMessage message, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, CoapMessage messageValue, ISourceBlock<CoapMessage>? source, bool consumeToAccept)
        {
            throw new NotImplementedException();
        }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public void Fault(Exception exception)
        {
            throw new NotImplementedException();
        }

        public Task Completion { get; }
        public IDisposable LinkTo(ITargetBlock<CoapMessageContext> target, DataflowLinkOptions linkOptions)
        {
            throw new NotImplementedException();
        }

        public CoapMessageContext? ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<CoapMessageContext> target, out bool messageConsumed)
        {
            throw new NotImplementedException();
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<CoapMessageContext> target)
        {
            throw new NotImplementedException();
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<CoapMessageContext> target)
        {
            throw new NotImplementedException();
        }

        public bool TryReceive(Predicate<CoapMessageContext>? filter, out CoapMessageContext item)
        {
            throw new NotImplementedException();
        }

        public bool TryReceiveAll(out IList<CoapMessageContext>? items)
        {
            throw new NotImplementedException();
        }
    }

    public class RequestHandler : IRequestHandler, ITargetBlock<CoapMessage>, IReceivableSourceBlock<CoapMessageContext>
    {
        private readonly TransformBlock<CoapMessage, CoapMessageContext> transformer;

        public RequestHandler()
        {
            this.transformer = new TransformBlock<CoapMessage, CoapMessageContext>(this.Transformer);
        }

        public bool CanHandle(CoapVersion version)
        {
            return version.Equals(CoapVersion.V1);
        }

        public async Task HandleAsync(CoapMessage message, CancellationToken ct)
        {
            await this.transformer.SendAsync(message, ct).ConfigureAwait(false);
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, CoapMessage messageValue, ISourceBlock<CoapMessage>? source, bool consumeToAccept)
        {
            throw new NotImplementedException();
        }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public void Fault(Exception exception)
        {
            throw new NotImplementedException();
        }

        public Task Completion { get; }

        public IDisposable LinkTo(ITargetBlock<CoapMessageContext> target, DataflowLinkOptions linkOptions)
        {
            throw new NotImplementedException();
        }

        public CoapMessageContext? ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<CoapMessageContext> target, out bool messageConsumed)
        {
            throw new NotImplementedException();
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<CoapMessageContext> target)
        {
            throw new NotImplementedException();
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<CoapMessageContext> target)
        {
            throw new NotImplementedException();
        }

        public bool TryReceive(Predicate<CoapMessageContext>? filter, out CoapMessageContext item)
        {
            throw new NotImplementedException();
        }

        public bool TryReceiveAll(out IList<CoapMessageContext>? items)
        {
            throw new NotImplementedException();
        }

        private async Task<CoapMessageContext> Transformer(CoapMessage message)
        {
            if (message.IsEmptyMessage)
            {

            }

            if (message.Header.Type.Equals(CoapType.Confirmable))
            {
                // Reliable Transmission

            }

            if (message.Header.Type.Equals(CoapType.NonConfirmable) && !message.Payload.IsEmpty)
            {
                // Transmission without Reliability
            }
        }
    }

    public class CoapMessageContext
    {
        public CoapMessageContext(CoapConnection connection, CoapMessage message)
        {
            Connection = connection;
            Message = message;
        }

        public CoapConnection Connection { get; }

        public CoapMessage Message { get; }
    }
}
