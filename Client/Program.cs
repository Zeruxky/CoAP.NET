using System;

namespace Client
{
    using CoAP;

    class Program
    {
        static void Main(string[] args)
        {
            var client = new CoapClient(new Uri("coap://127.0.0.1:5683/a/b/c"));
            client.Get();
        }
    }
}
