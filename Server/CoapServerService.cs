namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using WorldDirect.CoAP;

    public class CoapServerService : BackgroundService
    {
        private readonly CoapServer server;
        private readonly ILogger<CoapServerService> logger;
        private readonly IEnumerable<IMessageSerializer> serializers;

        public CoapServerService(CoapServer server, ILogger<CoapServerService> logger, IEnumerable<IMessageSerializer> serializers)
        {
            this.server = server;
            this.logger = logger;
            this.serializers = serializers;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation($"Started CoAP server at {server.LocalEndPoint}.");
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                {
                    var combinedCt = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, cts.Token);
                    var result = await this.server.ReceiveAsync(combinedCt.Token).ConfigureAwait(false);
                    this.logger.LogDebug($"Received {result.Buffer.Length} bytes from {result.RemoteEndPoint}.");
                    var serializer = this.serializers.Single(s => s.CanDeserialize(result));
                    var message = serializer.Deserialize(result.Buffer);
                    this.logger.LogInformation($"Deserialized message with ID {message.Header.Id} and token {message.Token}.");
                }
            }
        }
    }
}