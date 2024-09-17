using Azure.Messaging;
using Azure.Messaging.ServiceBus;
using Bridge.Bus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

internal class AzureServiceBusMessageHandler : IBrokerMessageHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Consumer _consumer;
    private readonly ILogger<AzureServiceBusMessageHandler> _logger;

    internal AzureServiceBusMessageHandler(
        IServiceProvider serviceProvider,
        Consumer consumer)
    {
        _serviceProvider = serviceProvider;
        _consumer = consumer;
        _logger = serviceProvider.GetRequiredService<ILogger<AzureServiceBusMessageHandler>>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var serviceBusClientFactory = _serviceProvider
            .GetRequiredService<IAzureClientFactory<ServiceBusProcessor>>();
        var client = serviceBusClientFactory
            .CreateClient(_consumer.QueueName);

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
        var cloudEvent = CloudEvent.Parse(arg.Message.Body);
        if (cloudEvent == null)
        {
            await arg.DeadLetterMessageAsync(arg.Message);
        }
        else
        {
            await _consumer.HandleMessage(_serviceProvider, cloudEvent, arg.CancellationToken);
            await arg.CompleteMessageAsync(arg.Message);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var serviceBusClientFactory = _serviceProvider
            .GetRequiredService<IAzureClientFactory<ServiceBusProcessor>>();
        var client = serviceBusClientFactory
            .CreateClient(_consumer.QueueName);

        client.ProcessMessageAsync -= HandleMessage;
        client.ProcessErrorAsync -= HandleError;

        await client.StopProcessingAsync(cancellationToken);
    }
}