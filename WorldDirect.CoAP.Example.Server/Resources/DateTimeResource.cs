namespace WorldDirect.CoAP.Example.Server.Resources
{
    using System;
    using System.Threading.Tasks;
    using CoAP.Server.Resources;
    using Quartz;
    using Quartz.Impl;

    public class DateTimeResource : Resource
    {
        public static async Task<DateTimeResource> Create()
        {
            var resource = new DateTimeResource("DateTime", true);
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler().ConfigureAwait(false);
            await scheduler.Start().ConfigureAwait(false);
            var dataMap = new JobDataMap
            {
                {nameof(DateTimeResource), resource}
            };
            var job = JobBuilder.Create<ExampleJob>()
                .WithIdentity(nameof(ExampleJob), "group1")
                .SetJobData(dataMap)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("EverySecondTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(t =>
                {
                    t.WithIntervalInSeconds(10);
                    t.RepeatForever();
                })
                .Build();

            await scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
            return resource;
        }

        private DateTimeResource(string name) : base(name)
        {
        }

        private DateTimeResource(string name, bool visible) : base(name, visible)
        {
            this.Attributes.Title = "GET the current time";
            this.Attributes.AddResourceType("CurrentTime");
            this.Observable = true;
        }

        public void SimulateChangingResource()
        {
            this.Changed();
        }

        protected override void DoGet(CoapExchange exchange)
        {
            var payload = DateTimeOffset.Now.ToString("R");
            exchange.Respond(StatusCode.Content, payload, MediaType.TextPlain);
        }
    }
}