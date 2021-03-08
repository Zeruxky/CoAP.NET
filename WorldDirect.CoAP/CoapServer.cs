namespace WorldDirect.CoAP
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class CoapServer
    {
        private readonly ILogger<CoapServer> logger;
        private readonly UdpClient socket;

        public CoapServer(ILogger<CoapServer> logger)
        {
            this.logger = logger;
            this.LocalEndPoint = new IPEndPoint(IPAddress.Loopback, 5683);
            this.socket = new UdpClient(this.LocalEndPoint);
        }

        public IPEndPoint LocalEndPoint { get; }

        public async Task<UdpReceiveResult> ReceiveAsync()
        {
            var result = await this.socket.ReceiveAsync().ConfigureAwait(false);
            this.logger.ReceivedMessage(result.Buffer.Length, result.RemoteEndPoint);
            return result;
        }
    }

    internal static class LoggingExtensions
    {
        private static readonly Action<ILogger, int, IPEndPoint, Exception> ReceivedMessageDelegate;

        static LoggingExtensions()
        {
            ReceivedMessageDelegate = LoggerMessage.Define<int, IPEndPoint>(
                LogLevel.Debug,
                new EventId(0),
                "Received {amountOfBytes} bytes from endpoint {remoteEndpoint}.");
        }

        internal static void ReceivedMessage(this ILogger<CoapServer> logger, int bytes, IPEndPoint remoteEndPoint)
            => ReceivedMessageDelegate(logger, bytes, remoteEndPoint, null);
    }
}
