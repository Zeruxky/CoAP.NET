namespace WorldDirect.CoAP.Example.Server.Resources
{
    using System;
    using System.IO;
    using System.Reflection.PortableExecutable;
    using System.Text;
    using CoAP.Server.Resources;

    public class ImageResource : Resource
    {
        private Int32[] _supported = new Int32[] {
            MediaType.ImageJpeg,
            MediaType.ImagePng
        };

        public ImageResource(String name)
            : this(name, false)
        {
        }

        public ImageResource(string name, bool visible)
            : base(name, visible)
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

        protected override void DoPost(CoapExchange exchange)
        {
            var request = exchange.Request;
            Directory.CreateDirectory("upload");
            File.WriteAllBytes("upload/upload.jpg", request.Payload);
            var response = new Response(StatusCode.Created)
            {
                LocationPath = "upload"
            };
            exchange.Respond(response);
        }
    }
}
