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

        var options = new AzureServiceBusOptions();
        configureOptions?.Invoke(options);

        foreach (var messageHandlerOption in builder.Consumers)
        {
            builder.Services.AddHostedService(sp => 
                new AzureServiceBusProcessor(sp, messageHandlerOption));
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

            foreach (var messageHandlerOption in builder.Consumers)
            {
                azureClientFactoryBuilder
                    .AddClient<ServiceBusProcessor, ServiceBusClientOptions>((_, _, provider) =>
                    {
                        var serviceBusClient = provider.GetRequiredService<ServiceBusClient>();
                        return serviceBusClient.CreateProcessor(messageHandlerOption.QueueName);
                    })
                    .WithName(messageHandlerOption.QueueName);
            }
        });
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