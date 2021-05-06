using System;

namespace Client
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CoAP;

    class Program
    {
        static async Task Main(string[] args)
        {
            var clients = Enumerable.Range(0, 10).Select(i => new CoapClient(new Uri($"coap://127.0.0.1:5683/{i}"))).ToArray();
            while (true)
            {
                var tasks = clients.Select(c => Task.Run(c.Get)).ToArray();
                await Task.WhenAll(tasks).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
        }
    }
}
