namespace WorldDirect.CoAP.Example.Server.Resources
{
    using System;
    using System.Threading.Tasks;
    using CoAP.Server.Resources;

    public class DateTimeResource : Resource
    {
        private readonly Task delay;
        private bool isObserved = true;
        
        public DateTimeResource(string name, bool visible) : base(name, visible)
        {
            this.Attributes.Title = "GET the current time";
            this.Attributes.AddResourceType("CurrentTime");
            this.Observable = true;
            this.delay = this.ChangeResourceAsync();
        }

        public async Task ChangeResourceAsync()
        {
            while (this.isObserved)
            {
                this.Changed();
                await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            }
        }

        protected override void DoGet(CoapExchange exchange)
        {
            if (exchange.Request.Observe == 1)
            {
                this.isObserved = false;
                this.ClearAndNotifyObserveRelations(StatusCode.Content);
            }

            var payload = DateTimeOffset.Now.ToString("R");
            exchange.Respond(StatusCode.Content, payload, MediaType.TextPlain);
        }
    }
}
