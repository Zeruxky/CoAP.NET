namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using WorldDirect.CoAP;

    public class CoapServerServiceOptions
    {
        public const string KEY = "CoapServerService";

        public string LocalAddress { get; set; } = IPAddress.Loopback.ToString();

        public int Port { get; set; } = 5683;
    }

    public class CoapServerService : BackgroundService
    {
        private readonly CoapServer server;
        private readonly ILogger<CoapServerService> logger;
        private readonly IEnumerable<IMessageSerializer> serializers;

        public CoapServerService(ILogger<CoapServerService> logger, IEnumerable<IMessageSerializer> serializers, IOptionsMonitor<CoapServerServiceOptions> options)
        {
            this.logger = logger;
            this.serializers = serializers;
            this.server = new CoapServer(IPAddress.Parse(options.CurrentValue.LocalAddress), options.CurrentValue.Port);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.StartedServer(this.server.LocalEndPoint);
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await this.server.ReceiveAsync().ConfigureAwait(false);
                this.logger.ReceivedMessage(result.RemoteEndPoint, result.Buffer.Length);
                var serializer = this.serializers.Single(s => s.CanDeserialize(result));
                var message = serializer.Deserialize(result.Buffer);
            }

            this.logger.StoppedServer();
        }
    }

    internal static class LoggingExtensions
    {
        private static readonly Action<ILogger, IPEndPoint, Exception> StartedServerDelegate;
        private static readonly Action<ILogger, Exception> StoppedServerDelegate;
        private static readonly Action<ILogger, IPEndPoint, long, Exception> ReceivedMessageDelegate;

        static LoggingExtensions()
        {
            ReceivedMessageDelegate = LoggerMessage.Define<IPEndPoint, long>(
                LogLevel.Debug,
                new EventId(0),
                "Received {bytes} bytes from endpoint {endpoint}.");

            StartedServerDelegate = LoggerMessage.Define<IPEndPoint>(
                LogLevel.Information,
                new EventId(0),
                "Started CoAP server at {endpoint}.");

            StoppedServerDelegate = LoggerMessage.Define(
                LogLevel.Information,
                new EventId(0),
                "Stopped CoAP server.");
        }

        public static void ReceivedMessage(this ILogger<CoapServerService> logger, IPEndPoint remoteEndPoint, long bytes)
            => ReceivedMessageDelegate(logger, remoteEndPoint, bytes, null);

        public static void StartedServer(this ILogger<CoapServerService> logger, IPEndPoint localEndpoint)
            => StartedServerDelegate(logger, localEndpoint, null);

        public static void StoppedServer(this ILogger<CoapServerService> logger)
            => StoppedServerDelegate(logger, null);
    }
}
