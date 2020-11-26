using System;

namespace WorldDirect.CoAP.Example.Client
{
    using System.IO;
    using System.Net.Cache;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            NLog.LogManager.GetCurrentClassLogger().Debug("Hello");

            try
            {
                await DoPut();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }


        private static async Task DoGet()
        {
            var req = Request.NewGet();
            req.URI = new Uri("coap://localhost");
            req.TimedOut += (sender, args) =>
            {
                Console.WriteLine("Timeout");
            };

            var client = new CoapClient();
            await client.SendAsync(req, CancellationToken.None);
        }

        private static async Task DoPut()
        {
            var client = new CoapClient();
            var req = Request.NewPut();

            req.TimedOut += (sender, eventArgs) =>
            {
                Console.WriteLine("Timeout");
            };

            req.MaxRetransmit = 2;
            req.URI = new Uri("coap://localhost/image");
            var payload = File.ReadAllBytes("C:\\dev\\src\\spikes\\FirmwareDeployer\\FirmwareDeployer\\bin\\Debug\\netcoreapp3.1\\aligned.bin");
            req.SetPayload(payload, MediaType.ApplicationOctetStream);


            var rsp = await client.SendAsync(req, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
