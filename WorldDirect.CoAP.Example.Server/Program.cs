using System;
using WorldDirect.CoAP.Server;

namespace WorldDirect.CoAP.Example.Server
{
    using System.IO;
    using CoAP.Server.Resources;

    class Program
    {
        static void Main(string[] args)
        {
            var server = new CoapServer();
            server.Add(new HelloWorldResource("HelloWorld"));
            server.Add(new ImageResource("ImageResource"));
            server.Start();
            Console.ReadLine();
        }
    }

    public class ImageResource : Resource
    {
        private Int32[] _supported = new Int32[] {
            MediaType.ImageJpeg,
            MediaType.ImagePng
        };

        public ImageResource(String name)
            : base(name)
        {
            Attributes.Title = "GET an image with different content-types";
            Attributes.AddResourceType("Image");

            foreach (Int32 item in _supported)
            {
                Attributes.AddContentType(item);
            }

            Attributes.MaximumSizeEstimate = 18029;
        }

        protected override void DoGet(CoapExchange exchange)
        {
            String file = "data\\image\\";
            Int32 ct = MediaType.ImagePng;
            Request request = exchange.Request;

            if ((ct = MediaType.NegotiationContent(ct, _supported, request.GetOptions(OptionType.Accept)))
                == MediaType.Undefined)
            {
                exchange.Respond(StatusCode.NotAcceptable);
            }
            else
            {
                file += "image." + MediaType.ToFileExtension(ct);
                if (File.Exists(file))
                {
                    Byte[] data = null;

                    try
                    {
                        data = File.ReadAllBytes(file);
                    }
                    catch (Exception ex)
                    {
                        exchange.Respond(StatusCode.InternalServerError, "IO error");
                        Console.WriteLine(ex.Message);
                    }

                    Response response = new Response(StatusCode.Content);
                    response.Payload = data;
                    response.ContentType = ct;
                    exchange.Respond(response);
                }
                else
                {
                    exchange.Respond(StatusCode.InternalServerError, "Image file not found");
                }
            }
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
