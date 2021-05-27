namespace WorldDirect.CoAP
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Codes;
    using V1;
    using V1.Messages;

    public interface IEndpoint
    {
        public string Address { get; }

        public int Port { get; }
    }

    public class CoapConnection
    {
        public CoapConnection(IEndpoint remoteEndpoint, IEndpoint localEndpoint)
        {
            RemoteEndpoint = remoteEndpoint;
            LocalEndpoint = localEndpoint;
        }

        public IEndpoint RemoteEndpoint { get; }

        public IEndpoint LocalEndpoint { get; }
    }

    public class RawCoapMessage
    {
        public RawCoapMessage(CoapConnection connection, ReadOnlyMemory<byte> content)
        {
            this.Connection = connection;
            this.Content = content;
        }

        public CoapConnection Connection { get; }

        public ReadOnlyMemory<byte> Content { get; }
    }

    public interface IChannel
    {
        IAsyncEnumerable<RawCoapMessage> ReceiveAsync(CancellationToken ct);

        Task SendAsync(RawCoapMessage context, CancellationToken ct);
    }

    public class UdpEndpoint : IEndpoint
    {
        public UdpEndpoint(IPEndPoint endpoint)
        {
            this.Address = endpoint.Address.ToString();
            this.Port = endpoint.Port;
        }

        public string Address { get; }

        public int Port { get; }
    }

    public class UdpChannel : IChannel
    {
        private readonly UdpClient client;

        public UdpChannel()
        {
            var ipEndpoint = new IPEndPoint(IPAddress.Loopback, 5683);
            this.Endpoint = new UdpEndpoint(ipEndpoint);
            this.client = new UdpClient(ipEndpoint);
        }

        public IEndpoint Endpoint { get; }

        public async IAsyncEnumerable<RawCoapMessage> ReceiveAsync([EnumeratorCancellation] CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var result = await this.client.ReceiveAsync().ConfigureAwait(false);
                var connection = new CoapConnection(new UdpEndpoint(result.RemoteEndPoint), this.Endpoint);
                var message = new RawCoapMessage(connection, result.Buffer);
                yield return message;
            }

            ct.ThrowIfCancellationRequested();
        }

        public Task SendAsync(RawCoapMessage context, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }

    public interface IMessageHandler
    {
        bool CanHandle(CoapMessage message);

        Task HandleAsync(CoapMessage message, CancellationToken ct);
    }

    public interface IRequestHandler
    {
        bool CanHandle(CoapVersion version);

        Task HandleAsync(CoapMessage message, CancellationToken ct);
    }

    public interface IResponseHandler
    {
        bool CanHandle(CoapVersion version);

        Task HandleAsync(CoapMessage message, CancellationToken ct);
    }

    public class MessageHandler : IMessageHandler, IReceivableSourceBlock<CoapMessage>
    {
        private readonly BufferBlock<CoapMessage> incomingMessages;

        public MessageHandler(IEnumerable<IRequestHandler> requestHandlers, IEnumerable<IResponseHandler> responseHandlers)
        {
            this.incomingMessages = new BufferBlock<CoapMessage>();
            var requestHandler = (RequestHandler)requestHandlers.Single(r => r.CanHandle(CoapVersion.V1));
            var responseHandler = (ResponseHandler)responseHandlers.Single(r => r.CanHandle(CoapVersion.V1));
            this.LinkTo(requestHandler, m => m.Header.Code is RequestCode);
            this.LinkTo(responseHandler, m => m.Header.Code is ResponseCode);
        }

        public bool CanHandle(CoapMessage message)
        {
            return message.Header.Version.Equals(CoapVersion.V1);
        }

        public async Task HandleAsync(CoapMessage message, CancellationToken ct)
        {
            await this.incomingMessages.SendAsync(message, ct).ConfigureAwait(false);
        }

        public void Complete()
        {
            this.incomingMessages.Complete();
        }

        public void Fault(Exception exception)
        {
            ((IDataflowBlock)this.incomingMessages).Fault(exception);
        }

        public Task Completion => this.incomingMessages.Completion;

        public IDisposable LinkTo(ITargetBlock<CoapMessage> target, DataflowLinkOptions linkOptions)
        {
            return this.incomingMessages.LinkTo(target, linkOptions);
        }

        public CoapMessage ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<CoapMessage> target, out bool messageConsumed)
        {
            return ((ISourceBlock<CoapMessage>)this.incomingMessages).ConsumeMessage(messageHeader, target, out messageConsumed);
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<CoapMessage> target)
        {
            return ((ISourceBlock<CoapMessage>)this.incomingMessages).ReserveMessage(messageHeader, target);
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<CoapMessage> target)
        {
            ((ISourceBlock<CoapMessage>)this.incomingMessages).ReleaseReservation(messageHeader, target);
        }

        public bool TryReceive(Predicate<CoapMessage> filter, out CoapMessage item)
        {
            return this.incomingMessages.TryReceive(filter, out item);
        }

        public bool TryReceiveAll(out IList<CoapMessage> items)
        {
            return this.incomingMessages.TryReceiveAll(out items);
        }
    }

    public class CoapServer
    {
        private readonly IEnumerable<IChannel> channels;
        private readonly IEnumerable<IMessageSerializer> serializers;
        private readonly IEnumerable<IMessageHandler> handlers;

        public CoapServer(IEnumerable<IChannel> channels, IEnumerable<IMessageSerializer> serializers, IEnumerable<IMessageHandler> handlers)
        {
            this.channels = channels;
            this.serializers = serializers;
            this.handlers = handlers;
        }

        public async Task RunAsync(CancellationToken ct)
        {
            var tasks = this.channels.Select(c => this.ReceiveMessageAsync(c, ct)).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task ReceiveMessageAsync(IChannel channel, CancellationToken ct)
        {
            await foreach (var rawMessage in channel.ReceiveAsync(ct).WithCancellation(ct).ConfigureAwait(false))
            {
                var message = this.SerializeMessage(rawMessage);
                await this.ProvideMessageToHandlerAsync(message, ct).ConfigureAwait(false);
            }
        }

        private async Task ProvideMessageToHandlerAsync(CoapMessage message, CancellationToken ct)
        {
            var handler = this.handlers.SingleOrDefault(h => h.CanHandle(message));
            if (handler == null)
            {
                throw new InvalidOperationException("No suitable handler found for given message.");
            }

            await handler.HandleAsync(message, ct).ConfigureAwait(false);
        }

        private CoapMessage SerializeMessage(RawCoapMessage rawMessage)
        {
            var serializer = this.serializers.SingleOrDefault(s => s.CanDeserialize(rawMessage.Content));
            if (serializer == null)
            {
                throw new InvalidOperationException("No suitable serializer found for given message.");
            }

            var message = serializer.Deserialize(rawMessage.Content);
            return message;
        }
    }
}
