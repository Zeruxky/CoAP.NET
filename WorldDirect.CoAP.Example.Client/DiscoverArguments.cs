namespace WorldDirect.CoAP.Example.Client
{
    using CommandLine;

    [Verb("discover", HelpText = "Sends a DISCOVER request to the given CoAP endpoint.")]
    public class DiscoverArguments
    {
        [Option('e', "endpoint", HelpText = "Sets the endpoint for this DISCOVER request.")]
        public string Endpoint { get; set; }
    }
}