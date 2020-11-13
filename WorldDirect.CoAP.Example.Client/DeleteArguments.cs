namespace WorldDirect.CoAP.Example.Client
{
    using CommandLine;

    [Verb("delete", HelpText = "Sends a delete request to the given CoAP endpoint.")]
    public class DeleteArguments
    {
        [Option('e', "endpoint", HelpText = "Sets the endpoint for this DELETE request.")]
        public string Endpoint { get; set; }
    }
}