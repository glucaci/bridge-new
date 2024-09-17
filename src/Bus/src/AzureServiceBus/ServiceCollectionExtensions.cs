using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Bridge.Bus;
using Microsoft.Extensions.Azure;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static BusBridgeBuilder UsingAzureServiceBus(
        this BusBridgeBuilder builder,
        Action<AzureServiceBusBrokerOptions>? configure = default)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var options = new AzureServiceBusBrokerOptions();
        configure?.Invoke(options);

        foreach (var messageHandlerOption in builder.Consumers)
        {
            builder.RegisterBrokerMessageHandler(sp => 
                new AzureServiceBusMessageHandler(sp, messageHandlerOption));
        }

        builder.Services.AddAzureClients(azureClientFactoryBuilder =>
        {
            if (options.AuthenticationType == AuthenticationType.AccessKeys)
            {
                azureClientFactoryBuilder
                    .AddServiceBusClient(options.ConnectionString)
                    .ConfigureOptions(ConfigureServiceBusClientOptions);
            }
            else
            {
                azureClientFactoryBuilder
                    .AddServiceBusClientWithNamespace(options.FullyQualifiedNamespace)
                    .ConfigureOptions(ConfigureServiceBusClientOptions);

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

    private static void ConfigureServiceBusClientOptions(ServiceBusClientOptions options)
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
    }
}