using System;

namespace Server
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;
    using WorldDirect.CoAP;
    using WorldDirect.CoAP.Codes;
    using WorldDirect.CoAP.V1;
    using WorldDirect.CoAP.V1.Messages;
    using WorldDirect.CoAP.V1.Options;

    public class X
    {
        private readonly IEnumerable<IMessageSerializer> serializers;
        private readonly TransformBlock<Task<UdpReceiveResult>, CoapMessage> block;

        public X(IEnumerable<IMessageSerializer> serializers)
        {
            this.serializers = serializers;
            this.block = new TransformBlock<Task<UdpReceiveResult>, CoapMessage>(this.XAsync);
        }

        public Task EnqueueAsync(Task<UdpReceiveResult> item, CancellationToken ct = default)
        {
            return this.block.SendAsync(item, ct);
        }

        public async Task<CoapMessage> GetMessageAsync(CancellationToken ct = default)
        {
            while (await this.block.OutputAvailableAsync(ct).ConfigureAwait(false))
            {
                var message = await this.block.ReceiveAsync(ct).ConfigureAwait(false);
                return message;
            }

            return default;
        }

        private async Task<CoapMessage> XAsync(Task<UdpReceiveResult> result)
        {
            var x = await result.ConfigureAwait(false);
            var serializer = this.serializers.Single(s => s.CanDeserialize(x));
            return serializer.Deserialize(x.Buffer);
        }
    }

    public class CoapServerService : BackgroundService
    {
        private readonly CoapServer server;
        private readonly ILogger<CoapServerService> logger;
        private readonly IEnumerable<IMessageSerializer> serializers;

        public CoapServerService(CoapServer server, ILogger<CoapServerService> logger, IEnumerable<IMessageSerializer> serializers)
        {
            this.server = server;
            this.logger = logger;
            this.serializers = serializers;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation($"Started CoAP server at {server.LocalEndPoint}.");
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                {
                    var combinedCt = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, cts.Token);
                    var result = await this.server.ReceiveAsync(combinedCt.Token).ConfigureAwait(false);
                    this.logger.LogDebug($"Received {result.Buffer.Length} bytes from {result.RemoteEndPoint}.");
                    var serializer = this.serializers.Single(s => s.CanDeserialize(result));
                    var message = serializer.Deserialize(result.Buffer);
                    this.logger.LogInformation($"Deserialized message with ID {message.Header.Id} and token {message.Token}.");
                }
            }
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
                    services.UseRFC7252Specification();
                });
        }
    }

    public static class ServiceCollectionExtensions
    {
        private static IServiceCollection AddCoapCodes(this IServiceCollection services)
        {
            return services.Scan(scan =>
            {
                scan.FromApplicationDependencies()
                    .AddClasses(c => c.Where(t => !t.IsEquivalentTo(typeof(UnknownCode)) && t.IsAssignableFrom(typeof(CoapCode))))
                    .As<CoapCode>()
                    .WithTransientLifetime();
            });
        }

        private static IServiceCollection AddReaders(this IServiceCollection services)
        {
            return services.Scan(scan =>
            {
                scan.FromApplicationDependencies()
                    .AddClasses(c => c.AssignableTo(typeof(IReader<>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime();
            });
        }

        public static IServiceCollection AddReaders(this IServiceCollection services, params Assembly[] assemblies)
        {
            return services.Scan(scan =>
            {
                scan.FromAssemblies(assemblies)
                    .AddClasses(c => c.AssignableTo(typeof(IReader<>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime();
            });
        }

        public static IServiceCollection AddCoapCodes(this IServiceCollection services, params Assembly[] assemblies)
        {
            return services.Scan(scan =>
            {
                scan.FromAssemblies(assemblies)
                    .AddClasses(c => c.AssignableTo<CoapCode>())
                    .As<CoapCode>()
                    .WithTransientLifetime();
            });
        }

        private static IServiceCollection AddOptionFactories(this IServiceCollection services)
        {
            return services.Scan(scan =>
            {
                scan.FromApplicationDependencies()
                    .AddClasses(c => c.AssignableTo<IOptionFactory>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime();
            });
        }

        public static IServiceCollection AddOptionFactories(this IServiceCollection services, params Assembly[] assemblies)
        {
            return services.Scan(scan =>
            {
                scan.FromAssemblies(assemblies)
                    .AddClasses(c => c.AssignableTo<IOptionFactory>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime();
            });
        }

        public static IServiceCollection AddContentFormats(this IServiceCollection services)
        {
            return services.Scan(scan =>
            {
                scan.FromApplicationDependencies()
                    .AddClasses(c => c.AssignableTo<ContentFormat>())
                    .As<ContentFormat>()
                    .WithTransientLifetime();
            });
        }

        public static IServiceCollection AddContentFormats(this IServiceCollection services, params Assembly[] assemblies)
        {
            return services.Scan(scan =>
            {
                scan.FromAssemblies(assemblies)
                    .AddClasses(c => c.AssignableTo<ContentFormat>())
                    .As<ContentFormat>()
                    .WithTransientLifetime();
            });
        }

        public static IServiceCollection UseRFC7252Specification(this IServiceCollection services)
        {
            services.AddTransient<IMessageSerializer, CoapMessageSerializer>(s =>
            {
                var codeRegistry = s.GetRequiredService<CodeRegistry>();
                var optionFactories = s.GetServices<IOptionFactory>();
                return new CoapMessageSerializer(new HeaderReader(codeRegistry), new TokenReader(), new OptionsReader(optionFactories), new PayloadReader());
            });

            services.AddCoapCodes();
            services.AddOptionFactories();
            services.AddTransient<CodeRegistry>();
            services.AddContentFormats();
            services.AddTransient<ContentFormatRegistry>();
            return services;
        }
    }
}
