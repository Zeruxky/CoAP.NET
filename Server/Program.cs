using System;

namespace Server
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;
    using WorldDirect.CoAP;
    using WorldDirect.CoAP.Codes;
    using WorldDirect.CoAP.Codes.MethodCodes;
    using WorldDirect.CoAP.V1;
    using WorldDirect.CoAP.V1.Messages;
    using WorldDirect.CoAP.V1.Options;


    public class CoapServerService : BackgroundService
    {
        private readonly CoapServer server;

        public CoapServerService(CoapServer server)
        {
            this.server = server;
        }
        
        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.server.ReceiveAsync(stoppingToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Represents the application startup configuration.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            var rootLogger = NLog.LogManager.LoadConfiguration("./Config/NLog.config").GetCurrentClassLogger();
            try
            {
                rootLogger.Trace("Application started");
                CreateHostBuilder(args)
                    .Build()
                    .Run();
            }
            catch (Exception e)
            {
                rootLogger.Error(e, "Application crashed.");
                throw;
            }
            finally
            {
                rootLogger.Trace("Application finished.");
                NLog.LogManager.Shutdown();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("Config/appsettings.json", false);
                    config.AddJsonFile($"Config/appsettings.{context.HostingEnvironment.EnvironmentName}.json", true);
                })
                .ConfigureLogging(log =>
                {
                    log.ClearProviders();
                    log.SetMinimumLevel(LogLevel.Trace);
                    log.AddNLog();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<CoapServerService>();
                    services.AddTransient<CoapServer>();
                    services.AddTransient<IMessageSerializer, CoapMessageSerializer>();
                    services.AddTransient<IReader<CoapHeader>, HeaderReader>();
                    services.AddTransient<IReader<CoapToken>, TokenReader>();
                    services.AddTransient<IReader<IReadOnlyCollection<CoapOption>>, OptionsReader>();
                    services.AddTransient<IReader<ReadOnlyMemory<byte>>, PayloadReader>();
                    //services.Scan(scan =>
                    //{
                    //    scan.FromAssemblyOf<CoapCode>()
                    //        .AddClasses(c => c.AssignableTo<CoapCode>())
                    //        .As<CoapCode>()
                    //        .WithTransientLifetime();
                    //});
                    services.AddTransient<CoapCode, Get>();
                    services.AddTransient<IOptionFactory, UriPathFactory>();
                    services.AddTransient<CodeRegistry>();
                });
        }
    }
}
