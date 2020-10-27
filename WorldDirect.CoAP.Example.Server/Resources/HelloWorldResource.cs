namespace WorldDirect.CoAP.Example.Server.Resources
{
    using CoAP.Server.Resources;

    public class HelloWorldResource : Resource
    {
        /// <inheritdoc />
        public HelloWorldResource(string name)
            : this(name, false)
        {
            
        }

        /// <inheritdoc />
        public HelloWorldResource(string name, bool visible)
            : base(name, visible)
        {
            Attributes.Title = "GET a friendly greeting!";
            Attributes.AddResourceType("HelloWorldDisplayer");
        }

        /// <inheritdoc />
        protected override void DoGet(CoapExchange exchange)
        {
            exchange.Respond(StatusCode.Content, "Hello World!");
        }
    }

    public class ObserveResource : Resource
    {
        public ObserveResource(string name)
            : this(name, false)
        {
            
        }

        public ObserveResource(string name, bool visible)
            : base(name, visible)
        {
            Attributes.Title = "GET a friendly greeting!";
            Attributes.AddResourceType("HelloWorldDisplayer");
        }
    }
}
