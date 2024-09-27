using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Bridge.Bus;
using Microsoft.Extensions.Azure;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static BusBridgeBuilder UsingAzureServiceBus(
        this BusBridgeBuilder builder,
        Action<AzureServiceBusOptions>? configureOptions = default,
        Action<ServiceBusClientOptions>? configureClient = default)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        AzureServiceBusOptions options = new AzureServiceBusOptions();
        configureOptions?.Invoke(options);

        foreach (ConsumerConfiguration consumerConfiguration in builder.Consumers)
        {
            builder.Services.AddHostedService(sp =>
                new AzureServiceBusProcessor(sp, consumerConfiguration));
        }

        builder.Services.AddAzureClients(azureClientFactoryBuilder =>
        {
            if (options.AuthenticationType == AuthenticationType.AccessKeys)
            {
                azureClientFactoryBuilder
                    .AddServiceBusClient(options.ConnectionString)
                    .ConfigureOptions(o => ConfigureServiceBusClientOptions(o, configureClient));
            }
            else
            {
                azureClientFactoryBuilder
                    .AddServiceBusClientWithNamespace(options.FullyQualifiedNamespace)
                    .ConfigureOptions(o => ConfigureServiceBusClientOptions(o, configureClient));

                azureClientFactoryBuilder.UseCredential(new ManagedIdentityCredential());
            }

            foreach (ConsumerConfiguration consumerConfiguration in builder.Consumers)
            {
                azureClientFactoryBuilder
                    .AddClient<ServiceBusProcessor, ServiceBusClientOptions>((_, _, provider) =>
                    {
                        ServiceBusClient serviceBusClient = provider.GetRequiredService<ServiceBusClient>();

                        return serviceBusClient.CreateProcessor(
                            consumerConfiguration.QueueName,
                            new ServiceBusProcessorOptions
                            {
                                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                                AutoCompleteMessages = false,
                                MaxAutoLockRenewalDuration = consumerConfiguration.MaxProcessingTime
                            });
                    })
                    .WithName(consumerConfiguration.QueueName);
            }
        });

        builder.Services.AddSingleton<IMessageBus, AzureServiceBusSender>();

        return builder;
    }

    private static void ConfigureServiceBusClientOptions(
        ServiceBusClientOptions options,
        Action<ServiceBusClientOptions>? configureClient)
    {
        options.TransportType = ServiceBusTransportType.AmqpWebSockets;
        options.EnableCrossEntityTransactions = true;
        options.RetryOptions = new ServiceBusRetryOptions
        {
            Mode = ServiceBusRetryMode.Exponential,
            MaxRetries = 3,
            Delay = TimeSpan.FromSeconds(1),
            MaxDelay = TimeSpan.FromSeconds(10)
        };

        configureClient?.Invoke(options);
    }
}