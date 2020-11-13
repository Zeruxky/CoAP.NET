using System;
using WorldDirect.CoAP.Server;

namespace WorldDirect.CoAP.Example.Server
{
    using System.Threading.Tasks;
    using Resources;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new CoapServer();
            server.Add(new HelloWorldResource("HelloWorld", true));
            server.Add(new FtpResource("Ftp", true));
            server.Add(new DateTimeResource("DateTime", true));
            server.Start();
            DoSomething();
            GC.Collect();
            Console.ReadLine();
        }

        private static void DoSomething()
        {
            var x = new FtpResource("Ftp", true);
        }
    }
}
