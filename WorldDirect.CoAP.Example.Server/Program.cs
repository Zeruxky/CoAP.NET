using System;
using WorldDirect.CoAP.Server;

namespace WorldDirect.CoAP.Example.Server
{
    using CoAP.Server.Resources;

    class Program
    {
        static void Main(string[] args)
        {
            var server = new CoapServer();
            server.Add(new HelloWorldResource("HelloWorld"));
            server.Start();
            Console.ReadLine();
        }
    }

    public class HelloWorldResource : Resource
    {
        /// <inheritdoc />
        public HelloWorldResource(string name) : base(name)
        {
            Attributes.Title = "GET a friendly greeting!";
            Attributes.AddResourceType("HelloWorldDisplayer");
        }

        /// <inheritdoc />
        public HelloWorldResource(string name, bool visible) : base(name, visible)
        {
        }

        /// <inheritdoc />
        protected override void DoGet(CoapExchange exchange)
        {
            exchange.Respond(StatusCode.Content, "Hello World!");
        }
    }
}
