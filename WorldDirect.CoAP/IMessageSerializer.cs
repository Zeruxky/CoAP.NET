// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Codes;
    using Microsoft.Extensions.Logging;
    using V1;
    using WorldDirect.CoAP.V1.Messages;

    /// <summary>
    /// Defines the functionality to serialize and deserialize a <see cref="CoapMessage"/>.
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Deserializes the given <paramref name="value"/> to a <see cref="CoapMessage"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlySpan{T}"/> of <see cref="byte"/>s for deserialization.</param>
        /// <returns>A <see cref="CoapMessage"/> that is equivalent to the given <paramref name="value"/>.</returns>
        CoapMessage Deserialize(ReadOnlyMemory<byte> value);

        /// <summary>
        /// Determines whether this instance can deserialize <see cref="CoapMessage"/>s with the specified <see cref="CoapVersion"/>.
        /// </summary>
        /// <param name="value">The version of the <see cref="CoapMessage"/>.</param>
        /// <returns>
        ///   <c>true</c> if this instance can deserialize the <see cref="CoapMessage"/> with that <see cref="CoapVersion"/>; otherwise, <c>false</c>.
        /// </returns>
        bool CanDeserialize(UdpReceiveResult value);
    }

    public class CoapServer
    {
        private readonly IEnumerable<IMessageSerializer> serializers;
        private readonly ILogger<CoapServer> logger;
        private readonly UdpClient socket;

        public CoapServer(IEnumerable<IMessageSerializer> serializers, ILogger<CoapServer> logger)
        {
            this.serializers = serializers;
            this.logger = logger;
            this.LocalEndPoint = new IPEndPoint(IPAddress.Loopback, 5683);
            this.socket = new UdpClient(this.LocalEndPoint);
        }

        public IPEndPoint LocalEndPoint { get; }

        public Task<UdpReceiveResult> ReceiveAsync(CancellationToken ct = default)
        {
            return this.socket.ReceiveAsync();
        }
    }
}
