namespace WorldDirect.CoAP
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class CoapServer
    {
        private readonly IEnumerable<IMessageSerializer> serializers;
        private readonly ILogger<CoapServer> logger;
        private readonly UdpClient socket;

        public CoapServer(IEnumerable<IMessageSerializer> serializers, ILogger<CoapServer> logger, IOptionsMonitor<CoapServerOptions> options)
        {
            this.serializers = serializers;
            this.logger = logger;
            this.LocalEndPoint = new IPEndPoint(options.CurrentValue.Address, options.CurrentValue.Port);
            this.socket = new UdpClient(this.LocalEndPoint);
        }

        public IPEndPoint LocalEndPoint { get; }

        public Task<UdpReceiveResult> ReceiveAsync(CancellationToken ct = default)
        {
            return this.socket.ReceiveAsync();
        }
    }
}