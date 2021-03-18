namespace Server
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using WorldDirect.CoAP;

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
}
