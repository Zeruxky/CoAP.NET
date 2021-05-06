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

    public class CoapServerService : IHostedService
    {
        private Task runningTask;
        private CancellationTokenSource cancellationTokenSource;
        private readonly CoapServer server;
        private readonly ILogger<CoapServerService> logger;

        public CoapServerService(ILogger<CoapServerService> logger, ChannelHandlerBlock channelHandlerBlock, CoapMessageHandlerBlock messageHandlerBlock, RequestBlockHandler requestBlock, ResponseBlockHandler responseBlock)
        {
            this.logger = logger;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.server = new CoapServer(channelHandlerBlock, messageHandlerBlock, requestBlock, responseBlock);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.cancellationTokenSource.Token))
            {
                this.runningTask = this.server.RunAsync(combinedCts.Token);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
