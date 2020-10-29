using System;
using WorldDirect.CoAP.Server;

namespace WorldDirect.CoAP.Example.Server
{
    using System.Threading.Tasks;
    using Resources;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var dateTimeResource = await DateTimeResource.Create().ConfigureAwait(false);
            var server = new CoapServer();
            server.Add(new HelloWorldResource("HelloWorld", true));
            server.Add(new ImageResource("ImageResource", true));
            server.Add(dateTimeResource);
            server.Start();
            Console.ReadLine();
        }
    }
}
