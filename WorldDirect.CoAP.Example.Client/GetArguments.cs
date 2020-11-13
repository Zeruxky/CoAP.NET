namespace WorldDirect.CoAP.Example.Client
{
    using CommandLine;

    [Verb("get", HelpText = "Sends a GET request to the given endpoint and if set the request will be repeated multiple times.")]
    public class GetArguments
    {
        [Option('e', "endpoint", HelpText = "Sets the endpoint for this GET request.")]
        public string Endpoint { get; set; }

        [Option('m', "multiple", HelpText = "Sets the variable that indicates if the request should be sent multiple times.")]
        public bool Looping { get; set; }
    }
}