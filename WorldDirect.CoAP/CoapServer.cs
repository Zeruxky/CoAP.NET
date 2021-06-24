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
    using Codes.MethodCodes;
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

    public interface ITransport
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

    public class ResourceId
    {

    }

    public class RequestMessage
    {
        public RequestCode Code { get; }

        public MethodCode Method { get; }

        public ResourceId Resource { get; }

        public bool IsInitialMessage { get; }
    }

    public interface IChannel
    {
        Task RunAsync(CancellationToken ct);

        IAsyncEnumerable<RequestMessage> ReceiveRequestMessagesAsync(CancellationToken ct);
    }

    public class UdpChannel : IChannel
    {
        private readonly UdpTransport transport;
        private readonly CoapMessageSerializer serializer;
        private readonly TransformManyBlock<RawCoapMessage, CoapMessage> serializerBlock;
        private readonly TransformManyBlock<CoapMessage, RequestMessage> requestHandlerBlock;
        private readonly TransformManyBlock<CoapMessage, ResponseMessage> responseHandlerBlock;

        public UdpChannel(UdpTransport transport, CoAP.V1.CoapMessageSerializer serializer)
        {
            this.transport = transport;
            this.serializer = serializer;
            this.serializerBlock = new TransformManyBlock<RawCoapMessage, CoapMessage>(this.SerializeAsync);
            this.requestHandlerBlock = new TransformManyBlock<CoapMessage, RequestMessage>(this.BuildRequestMessage);
            this.responseHandlerBlock = new TransformManyBlock<CoapMessage, ResponseMessage>(this.BuildResponseMessage);
            this.serializerBlock.LinkTo(this.requestHandlerBlock, m => m.Header.Code is RequestCode);
            this.serializerBlock.LinkTo(this.responseHandlerBlock, m => m.Header.Code is ResponseCode);
        }

        public async Task RunAsync(CancellationToken ct)
        {
            await foreach (var rawMessage in this.transport.ReceiveAsync(ct).WithCancellation(ct).ConfigureAwait(false))
            {
                await this.serializerBlock.SendAsync(rawMessage, ct).ConfigureAwait(false);
            }
        }

        public async IAsyncEnumerable<RequestMessage> ReceiveRequestMessagesAsync([EnumeratorCancellation] CancellationToken ct)
        {
            while (await this.requestHandlerBlock.OutputAvailableAsync(ct).ConfigureAwait(false))
            {
                var request = await this.requestHandlerBlock.ReceiveAsync(ct).ConfigureAwait(false);
                yield return request;
            }
        }

        private async Task<IEnumerable<CoapMessage>> SerializeAsync(RawCoapMessage rawMessage)
        {
            try
            {
                var message = this.serializer.Deserialize(rawMessage.Content);
                return new[]
                {
                    message,
                };
            }
            catch (Exception e)
            {
                return Enumerable.Empty<CoapMessage>();
            }
        }

        private async Task<IEnumerable<RequestMessage>> BuildRequestMessage(CoapMessage message)
        {

        }

        private async Task<IEnumerable<ResponseMessage>> BuildResponseMessage(CoapMessage message)
        {
            return Enumerable.Empty<ResponseMessage>();
        }
    }

    public class UdpTransport : ITransport
    {
        private readonly UdpClient client;

        public UdpTransport()
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

    public class ResponseMessage
    {
        public RequestMessage Request { get; }

        public ResponseCode Code { get; }
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

        private async Task ReceiveMessageAsync(IChannel transport, CancellationToken ct)
        {
            var task = transport.RunAsync(ct);
            await foreach (var requestMessage in transport.ReceiveRequestMessagesAsync(ct).WithCancellation(ct).ConfigureAwait(false))
            {
                var x = requestMessage;
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
