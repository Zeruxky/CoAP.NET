using System;
using WorldDirect.CoAP.Server;

namespace WorldDirect.CoAP.Example.Server
{
    using Resources;

    class Program
    {
        static void Main(string[] args)
        {
            var server = new CoapServer();
            server.Add(new HelloWorldResource("HelloWorld", true));
            server.Add(new ImageResource("ImageResource", true));
            server.Start();
            Console.ReadLine();
        }
    }
}
