namespace Server
{
    using System;
    using System.Net;
    using Microsoft.Extensions.Logging;

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