namespace WorldDirect.CoAP.Example.Server.Resources
{
    using System.Threading.Tasks;
    using Quartz;

    public class ExampleJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var resource = (DateTimeResource)dataMap.Get(nameof(DateTimeResource));
            resource.SimulateChangingResource();
            return Task.CompletedTask;
        }
    }
}