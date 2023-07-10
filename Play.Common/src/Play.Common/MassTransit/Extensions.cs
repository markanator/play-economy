using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MassTransit;
using MassTransit.Definition;
using Play.Common.Settings;
using System;
using GreenPipes;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using GreenPipes.Configurators;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMQ(
            this IServiceCollection services,
            Action<IRetryConfigurator> configureRetries = null
            )
        {
            services.AddMassTransit(configX =>
            {
                // add all consumers in the entry assembly
                configX.AddConsumers(Assembly.GetEntryAssembly());
                // establish connection to Rabbitmq, with optional retriesConfigurator
                configX.UsingPlayEconomyRabbitMQ(configureRetries);
            });
            // start the bus, to send messages
            services.AddMassTransitHostedService();
            return services;
        }

        public static void UsingPlayEconomyRabbitMQ(
            this IServiceCollectionBusConfigurator configX,
            Action<IRetryConfigurator> configureRetries = null
            )
        {
            configX.UsingRabbitMq((ctx, cfgor) =>
            {
                var configuration = ctx.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                cfgor.Host(rabbitMQSettings.Host);
                // define how queue is names
                cfgor.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                // configure retries default settings
                if (configureRetries is null)
                {
                    configureRetries = (retryConfigurator) => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                }
                cfgor.UseMessageRetry(configureRetries);
            });
        }
    }
}