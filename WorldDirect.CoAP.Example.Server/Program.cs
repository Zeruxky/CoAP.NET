using System;

namespace WorldDirect.CoAP.Example.Server
{
    using System.IO;
    using WorldDirect.CoAP.Server;
    using WorldDirect.CoAP.Server.Resources;

    class Program
    {
        static void Main(string[] args)
        {
            var server = new CoapServer();
            server.Add(new ImageResource());
            server.Start();

            Console.WriteLine("Server started");
            Console.ReadLine();
        }
    }

    public class ImageResource : Resource
    {
        public ImageResource() : base("image", true)
        {
        }

        protected override void DoGet(CoapExchange exchange)
        {
            exchange.Request.Responding += (sender, args) =>
            {
                Console.WriteLine("Buh");
            };

            var payload = File.ReadAllBytes("C:\\dev\\src\\spikes\\FirmwareDeployer\\FirmwareDeployer\\bin\\Debug\\netcoreapp3.1\\aligned.bin");
            exchange.Respond(StatusCode.Content, payload);
            Console.WriteLine("Done");
        }

        protected override void DoPut(CoapExchange exchange)
        {
            Console.WriteLine("Received");
            exchange.Accept();
        }
    }
}
