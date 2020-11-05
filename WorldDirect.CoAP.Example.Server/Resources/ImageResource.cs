namespace WorldDirect.CoAP.Example.Server.Resources
{
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using CoAP.Server.Resources;

    public class FtpResource : Resource
    {
        private const string FilenamePattern = @"(?<content>.*?)--(?<filename>.*?)--";
        private string filename;

        public FtpResource(string name, bool visible)
            : base(name, visible)
        {
            Attributes.Title = "Resource for offering FTP service.";
        }

        protected override void DoGet(CoapExchange exchange)
        {
            var content = File.ReadAllBytes($"upload/{this.filename}");
            var extension = MediaType.Parse(this.filename.Substring(this.filename.IndexOf('.') + 1));
            var response = new Response(StatusCode.Content)
            {
                Accept = (int)OptionType.Accept,
            };

            response.SetPayload(content, extension);
            exchange.Respond(response);
        }

        protected override void DoPost(CoapExchange exchange)
        {
            var request = exchange.Request;
            var matchResult = Regex.Match(request.PayloadString, FilenamePattern);
            var tmpFilename = matchResult.Groups["filename"].Value;
            var filenameWithoutExtension = tmpFilename.Split('.')[0];
            var content = matchResult.Groups["content"].Value;
            File.WriteAllBytes($"upload/{tmpFilename}", Encoding.UTF8.GetBytes(content));
            var childResource = (Resource)this.GetChild(filenameWithoutExtension);
            Response response;
            if (childResource == null)
            {
                var resource = new FtpResource(filenameWithoutExtension, true)
                {
                    filename = tmpFilename,
                };
                this.Add(resource);
                response = new Response(StatusCode.Created)
                {
                    LocationPath = resource.Path,
                    UriPath = resource.Uri,
                };
            }
            else
            {
                childResource.Changed();
                response = new Response(StatusCode.Changed)
                {
                    LocationPath = childResource.Path,
                    UriPath = childResource.Uri,
                };
            }

            exchange.Respond(response);
        }
    }
}
