namespace WorldDirect.CoAP.V1
{
    using System;
    using Microsoft.Extensions.Logging;
    using WorldDirect.CoAP.V1.Messages;

    /// <summary>
    /// Contains extension methods for logging.
    /// </summary>
    internal static class LoggingExtensions
    {
        private static readonly Action<ILogger, CoapMessageId, CoapToken, int, Exception> SuccessfullyDeserializedMessageDelegate;

        static LoggingExtensions()
        {
            SuccessfullyDeserializedMessageDelegate = LoggerMessage.Define<CoapMessageId, CoapToken, int>(
                LogLevel.Debug,
                new EventId(0),
                "Deserialized message with ID {id}, token {token} and size of {bytes} bytes.");
        }

        /// <summary>
        /// Logs a message, if the <see cref="CoapMessage"/> was successfully deserialized.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <param name="bytes">The bytes.</param>
        internal static void SuccessfullyDeserializedMessage(this ILogger<CoapMessageSerializer> logger, CoapMessageId id, CoapToken token, int bytes)
            => SuccessfullyDeserializedMessageDelegate(logger, id, token, bytes, null);
    }
}
