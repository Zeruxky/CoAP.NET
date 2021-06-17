namespace Server
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using WorldDirect.CoAP;
    using WorldDirect.CoAP.Codes;
    using WorldDirect.CoAP.Codes.Common;
    using WorldDirect.CoAP.Codes.MethodCodes;
    using WorldDirect.CoAP.Common;
    using WorldDirect.CoAP.V1;
    using WorldDirect.CoAP.V1.Options;

    public static class ServiceCollectionExtensions
    {
        private static IServiceCollection AddCoapCodes(this IServiceCollection services)
        {
            return services.AddCoapCodes(typeof(Get).Assembly);
        }

        private static IServiceCollection AddReaders(this IServiceCollection services)
        {
            return services.AddReaders(typeof(HeaderReader).Assembly);
        }

        public static IServiceCollection AddReader<TService, TImplementation>(this IServiceCollection services)
        {
            return services.AddReader(typeof(TService), typeof(TImplementation));
        }

        public static IServiceCollection AddReader(this IServiceCollection services, Type service, Type implementation)
        {
            services.TryAddTransient(service, implementation);
            return services;
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
                    .AddClasses(c => c.AssignableTo<CoapCode>().Where(t => t != typeof(UnknownCode)))
                    .As<CoapCode>()
                    .WithTransientLifetime();
            });
        }

        public static IServiceCollection AddCoapCode<TCode>(this IServiceCollection services)
        {
            return services.AddCoapCode(typeof(TCode));
        }

        public static IServiceCollection AddCoapCode(this IServiceCollection services, Type type)
        {
            services.TryAddTransient(typeof(CoapCode), type);
            return services;
        }

        private static IServiceCollection AddOptionFactories(this IServiceCollection services)
        {
            return services.AddOptionFactories(typeof(Accept).Assembly);
        }

        public static IServiceCollection AddOptionFactory<TOptionFactory>(this IServiceCollection services)
        {
            return services.AddOptionFactory(typeof(TOptionFactory));
        }

        public static IServiceCollection AddOptionFactory(this IServiceCollection services, Type type)
        {
            services.TryAddTransient(typeof(IOptionFactory), type);
            return services;
        }

        public static IServiceCollection AddOptionFactories(this IServiceCollection services, params Assembly[] assemblies)
        {
            return services.Scan(scan =>
            {
                scan.FromAssemblies(assemblies)
                    .AddClasses(c => c.AssignableTo<IOptionFactory>().Where(t => t != typeof(UnrecognizedOptionFactory)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime();
            });
        }

        public static IServiceCollection AddContentFormats(this IServiceCollection services)
        {
            return services.AddContentFormats(typeof(JsonFormat).Assembly);
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

        public static IServiceCollection AddRegistry<TRegistry>(this IServiceCollection services)
        {
            return services.AddRegistry(typeof(TRegistry));
        }

        public static IServiceCollection AddRegistry(this IServiceCollection services, Type type)
        {
            services.TryAddTransient(type);
            return services;
        }

        private static IServiceCollection AddRegistries(this IServiceCollection services)
        {
            return services.AddRegistries(typeof(CodeRegistry).Assembly);
        }

        private static IServiceCollection AddRegistries(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.Scan(scan =>
            {
                scan.FromAssemblies(assemblies)
                    .AddClasses(c => c.AssignableTo(typeof(Registry<>)))
                    .AsSelf()
                    .WithTransientLifetime();
            });

            return services;
        }

        public static IServiceCollection AddChannels(this IServiceCollection services)
        {
            return services.AddChannels(typeof(UdpTransport).Assembly);
        }

        public static IServiceCollection AddChannels(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.Scan(scan =>
            {
                scan.FromAssemblies(assemblies)
                    .AddClasses(c => c.AssignableTo<ITransport>())
                    .As<ITransport>()
                    .WithSingletonLifetime();
            });

            return services;
        }

        public static IServiceCollection UseRFC7252Specification(this IServiceCollection services)
        {
            services.AddTransient(s =>
            {
                var codeRegistry = s.GetRequiredService<CodeRegistry>();
                var logger = s.GetRequiredService<ILogger<CoapMessageSerializer>>();
                var optionFactories = s.GetServices<IOptionFactory>();
                return new CoapMessageSerializer(new HeaderReader(codeRegistry), new TokenReader(), new OptionsReader(optionFactories), new PayloadReader(), logger);
            });

            services.AddCoapCodes();
            services.AddOptionFactories();
            services.AddRegistries();
            services.AddContentFormats();
            services.AddSingleton<CoapServer>();
            services.AddTransient<IChannel, UdpChannel>();
            services.AddTransient<UdpTransport>();
            return services;
        }
    }
}
