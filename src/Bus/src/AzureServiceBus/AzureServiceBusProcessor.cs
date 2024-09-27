using Azure.Messaging;
using Azure.Messaging.ServiceBus;
using Bridge.Bus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

internal class AzureServiceBusProcessor : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerConfiguration _consumerConfiguration;
    private readonly ILogger<AzureServiceBusProcessor> _logger;

    internal AzureServiceBusProcessor(
        IServiceProvider serviceProvider,
        ConsumerConfiguration consumerConfiguration)
    {
        _serviceProvider = serviceProvider;
        _consumerConfiguration = consumerConfiguration;
        _logger = serviceProvider.GetRequiredService<ILogger<AzureServiceBusProcessor>>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ServiceBusProcessor client = CreateServiceBusProcessor();

        client.ProcessMessageAsync += HandleMessage;
        client.ProcessErrorAsync += HandleError;

        await client.StartProcessingAsync(cancellationToken);
    }

    private Task HandleError(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "Error processing on entity: {EntityPath}", arg.EntityPath);
        return Task.CompletedTask;
    }

    private async Task HandleMessage(ProcessMessageEventArgs arg)
    {
        CloudEvent? cloudEvent = CloudEvent.Parse(arg.Message.Body);
        if (cloudEvent == null)
        {
            await arg.DeadLetterMessageAsync(arg.Message);
        }
        else
        {
            await _consumerConfiguration.HandleMessage(_serviceProvider, cloudEvent, arg.CancellationToken);
            await arg.CompleteMessageAsync(arg.Message);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ServiceBusProcessor client = CreateServiceBusProcessor();

        client.ProcessMessageAsync -= HandleMessage;
        client.ProcessErrorAsync -= HandleError;

        await client.StopProcessingAsync(cancellationToken);
        await client.DisposeAsync();
    }

    private ServiceBusProcessor CreateServiceBusProcessor()
    {
        IAzureClientFactory<ServiceBusProcessor> serviceBusClientFactory = _serviceProvider
            .GetRequiredService<IAzureClientFactory<ServiceBusProcessor>>();

        return serviceBusClientFactory
            .CreateClient(_consumerConfiguration.QueueName);
    }
}
