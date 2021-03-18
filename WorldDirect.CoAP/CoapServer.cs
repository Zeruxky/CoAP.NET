﻿namespace WorldDirect.CoAP
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Codes;
    using Microsoft.Extensions.Logging;
    using V1.Messages;
    using V1.Options;

    public class CoapServer
    {
        private readonly UdpClient socket;

        public CoapServer(IPAddress address, int port)
        {
            this.LocalEndPoint = new IPEndPoint(address, port);
            this.socket = new UdpClient(this.LocalEndPoint);
        }

        public IPEndPoint LocalEndPoint { get; }

        public Task<UdpReceiveResult> ReceiveAsync()
        {
            return this.socket.ReceiveAsync();
        }

        public void StopServer()
        {
            this.socket.Close();
        }
    }

    //public class CoapContext
    //{
    //    public ConnectionInfo Connection { get; set; }

    //    public CoapRequest Request { get; set; }
    //}

    //public class ConnectionInfo
    //{

    //}

    //public class CoapRequest
    //{
    //    public CoapCode Code { get; }

    //    public Uri RequestUri { get; }

    //    public CoapVersion Version => this.Message.Header.Version;

    //    public CoapType Type => this.Message.Header.Type;

    //    public CoapToken Token => this.Message.Token;

    //    public CoapMessage Message { get; }
    //}
}
