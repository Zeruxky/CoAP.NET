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

    public class ResponseBlockHandler : IBlockHandler<CoapMessage, CoapMessageContext>
    {
        public ResponseBlockHandler()
        {
            this.Input = new BufferBlock<CoapMessage>();
            this.Output = new BufferBlock<CoapMessageContext>();
        }

        public async Task RunAsync(CancellationToken ct)
        {
            await foreach (var message in this.Input.GetAllAsync(ct).WithCancellation(ct).ConfigureAwait(false))
            {
                this.ReceivedItems++;
                Console.WriteLine($"Received message with id {message.Header.Id} and token {message.Token} with response code {message.Header.Code}.");
                this.ProcessedItems++;
            }
        }

        public long ProcessedItems { get; private set; }

        public BufferBlock<CoapMessageContext> Output { get; }

        public IDisposable LinkTo(IReceiverBlock<CoapMessageContext> block)
        {
            return this.LinkTo(block, m => true);
        }

        public IDisposable LinkTo(IReceiverBlock<CoapMessageContext> block, Predicate<CoapMessageContext> filter)
        {
            var options = new DataflowLinkOptions() {PropagateCompletion = true,};
            return this.Output.LinkTo(block.Input, options, filter);
        }

        public long ReceivedItems { get; private set; }

        public BufferBlock<CoapMessage> Input { get; }
    }

    public class RequestBlockHandler : IBlockHandler<CoapMessage, CoapMessageContext>
    {
        public RequestBlockHandler()
        {
            this.Input = new BufferBlock<CoapMessage>();
            this.Output = new BufferBlock<CoapMessageContext>();
        }

        public async Task RunAsync(CancellationToken ct)
        {
            await foreach (var message in this.Input.GetAllAsync(ct).WithCancellation(ct).ConfigureAwait(false))
            {
                this.ReceivedItems++;
                Console.WriteLine($"Received message with id {message.Header.Id} and token {message.Token} with request code {message.Header.Code}.");
                this.ProcessedItems++;
            }
        }

        public long ProcessedItems { get; private set; }

        public BufferBlock<CoapMessageContext> Output { get; }

        public IDisposable LinkTo(IReceiverBlock<CoapMessageContext> block)
        {
            return this.LinkTo(block, ctx => true);
        }

        public IDisposable LinkTo(IReceiverBlock<CoapMessageContext> block, Predicate<CoapMessageContext> filter)
        {
            var options = new DataflowLinkOptions()
            {
                PropagateCompletion = true,
            };
            return this.Output.LinkTo(block.Input, options, filter);
        }

        public long ReceivedItems { get; private set; }

        public BufferBlock<CoapMessage> Input { get; }
    }

    public class CoapServer
    {
        private readonly List<IBlockHandler> blockHandlers;

        public CoapServer(IProcessorBlock<RawCoapMessage> channelBlock, IBlockHandler<RawCoapMessage, CoapMessage> serializerBlock, IReceiverBlock<CoapMessage> requestBlock, IReceiverBlock<CoapMessage> responseBlock)
        {
            channelBlock.LinkTo(serializerBlock);
            serializerBlock.LinkTo(requestBlock, m => m.Header.Code is RequestCode);
            serializerBlock.LinkTo(responseBlock, m => m.Header.Code is ResponseCode);
            this.blockHandlers = new List<IBlockHandler>()
            {
                channelBlock,
                serializerBlock,
                requestBlock,
                responseBlock,
            };
        }

        public async Task RunAsync(CancellationToken ct)
        {
            var tasks = this.blockHandlers.Select(h => h.RunAsync(ct)).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }

    internal static class SourceBlockExtensions
    {
        public static async IAsyncEnumerable<T> GetAllAsync<T>(this ISourceBlock<T> source, [EnumeratorCancellation] CancellationToken ct)
        {
            while (await source.OutputAvailableAsync(ct).ConfigureAwait(false))
            {
                yield return await source.ReceiveAsync(ct).ConfigureAwait(false);
            }

            ct.ThrowIfCancellationRequested();
        }
    }

    public abstract class ConfigurablePropagatorBlock<TInput, TOutput> : IPropagatorBlock<TInput, TOutput>
    {
        private readonly BufferBlock<TInput> input;
        private readonly BufferBlock<TOutput> output;

        protected ConfigurablePropagatorBlock(Action<ISourceBlock<TInput>, ITargetBlock<TOutput>> configure)
        {
            this.input = new BufferBlock<TInput>();
            this.output = new BufferBlock<TOutput>();
            configure(this.input, this.output);
        }

        public long ReceivedItems { get; private set; }

        public long ProcessedItems { get; private set; }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept)
        {
            this.ReceivedItems++;
            return ((ITargetBlock<TInput>)this.input).OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        public void Complete()
        {
            this.input.Complete();
            this.output.Complete();
        }

        public void Fault(Exception exception)
        {
            ((IDataflowBlock)this.input).Fault(exception);
            ((IDataflowBlock)this.output).Fault(exception);
        }

        public Task Completion => Task.WhenAll(this.input.Completion, this.output.Completion);

        public IDisposable LinkTo(ITargetBlock<TOutput> target, DataflowLinkOptions linkOptions)
        {
            return this.output.LinkTo(target, linkOptions);
        }

        public TOutput ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target, out bool messageConsumed)
        {
            var message = ((ISourceBlock<TOutput>)this.output).ConsumeMessage(messageHeader, target, out messageConsumed);
            if (messageConsumed)
            {
                this.ProcessedItems++;
            }

            return message;
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
        {
            return ((ISourceBlock<TOutput>)this.output).ReserveMessage(messageHeader, target);
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
        {
            ((ISourceBlock<TOutput>)this.output).ReleaseReservation(messageHeader, target);
        }
    }

    public class CoapMessageHandler : ConfigurablePropagatorBlock<RawCoapMessage, CoapMessage>
    {
        public CoapMessageHandler(IEnumerable<IMessageSerializer> serializers)
            : base((input, output) => Configure(input, output, serializers))
        {
        }

        private static void Configure(ISourceBlock<RawCoapMessage> input, ITargetBlock<CoapMessage> output, IEnumerable<IMessageSerializer> serializers)
        {
            foreach (var serializer in serializers)
            {
                var transformBlock = new TransformBlock<RawCoapMessage, CoapMessage>(r => serializer.Deserialize(r.Content));
                input.LinkTo(transformBlock, r => serializer.CanDeserialize(r.Content));
                transformBlock.LinkTo(output);
            }
        }
    }

    public class CoapMessageHandlerBlock : IBlockHandler<RawCoapMessage, CoapMessage>
    {
        private readonly IEnumerable<IMessageSerializer> serializers;

        public CoapMessageHandlerBlock(IEnumerable<IMessageSerializer> serializers)
        {
            this.Input = new BufferBlock<RawCoapMessage>();
            this.Output = new BufferBlock<CoapMessage>();
        }

        public long ReceivedItems { get; private set; }

        public long ProcessedItems { get; private set; }

        public BufferBlock<CoapMessage> Output { get; }

        public IDisposable LinkTo(IReceiverBlock<CoapMessage> block)
        {
            return this.LinkTo(block, r => true);
        }

        public IDisposable LinkTo(IReceiverBlock<CoapMessage> block, Predicate<CoapMessage> filter)
        {
            var options = new DataflowLinkOptions() { PropagateCompletion = true, };
            return this.Output.LinkTo(block.Input, options, filter);
        }

        public BufferBlock<RawCoapMessage> Input { get; }

        public async Task RunAsync(CancellationToken ct)
        {
            await foreach (var item in this.Input.GetAllAsync(ct).WithCancellation(ct).ConfigureAwait(false))
            {
                this.ReceivedItems++;
                var serializer = this.serializers.SingleOrDefault(s => s.CanDeserialize(item.Content));
                if (serializer != null)
                {
                    var message = serializer.Deserialize(item.Content);
                    await this.Output.SendAsync(message, ct).ConfigureAwait(false);
                    this.ProcessedItems++;
                }
            }
        }
    }

    public class ChannelHandlerBlock : IProcessorBlock<RawCoapMessage>
    {
        private readonly IEnumerable<IChannel> channels;

        public ChannelHandlerBlock(IEnumerable<IChannel> channels)
        {
            this.channels = channels;
            this.Output = new BufferBlock<RawCoapMessage>();
        }

        public async Task RunAsync(CancellationToken ct)
        {
            var tasks = this.channels.Select(c => this.ProcessIncomingPacketAsync(c, ct)).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public long ProcessedItems { get; private set; }

        public BufferBlock<RawCoapMessage> Output { get; }

        public IDisposable LinkTo(IReceiverBlock<RawCoapMessage> block)
        {
            return this.LinkTo(block, r => true);
        }

        public IDisposable LinkTo(IReceiverBlock<RawCoapMessage> block, Predicate<RawCoapMessage> filter)
        {
            var options = new DataflowLinkOptions() { PropagateCompletion = true, };
            return this.Output.LinkTo(block.Input, options, filter);
        }

        private async Task ProcessIncomingPacketAsync(IChannel channel, CancellationToken ct)
        {
            await foreach (var packet in channel.ReceiveAsync(ct).WithCancellation(ct).ConfigureAwait(false))
            {
                await this.Output.SendAsync(packet, ct).ConfigureAwait(false);
                this.ProcessedItems++;
            }
        }
    }

    public interface IBlockHandler
    {
        Task RunAsync(CancellationToken ct);
    }

    public interface IReceiverBlock<TReceived> : IBlockHandler
    {
        long ReceivedItems { get; }

        BufferBlock<TReceived> Input { get; }
    }

    public interface IProcessorBlock<TProcessed> : IBlockHandler
    {
        long ProcessedItems { get; }

        BufferBlock<TProcessed> Output { get; }

        IDisposable LinkTo(IReceiverBlock<TProcessed> block);

        IDisposable LinkTo(IReceiverBlock<TProcessed> block, Predicate<TProcessed> filter);
    }

    public interface IBlockHandler<TReceived, TProcessed> : IProcessorBlock<TProcessed>, IReceiverBlock<TReceived>
    {
    }
}
