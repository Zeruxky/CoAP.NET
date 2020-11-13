namespace WorldDirect.CoAP.Example.Client
{
    using CommandLine;

    [Verb("post", HelpText = "Sends a POST request to the given CoAP endpoint with the given payload.")]
    public class PostArguments
    {
        [Option('e', "endpoint", HelpText = "Sets the endpoint for this POST request.")]
        public string Endpoint { get; set; }

        [Option('p', "payload", HelpText = "Sets the payload for this POST request.")]
        public string Payload { get; set; }
    }
}