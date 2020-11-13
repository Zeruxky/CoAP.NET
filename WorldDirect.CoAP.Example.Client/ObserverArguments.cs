namespace WorldDirect.CoAP.Example.Client
{
    using CommandLine;

    [Verb("observe", HelpText = "Sends a OBSERVER request to the given CoAP endpoint.")]
    public class ObserverArguments
    {
        [Option('e', "endpoint", HelpText = "Sets the endpoint for this OBSERVE request.")]
        public string Endpoint { get; set; }
    }
}