using Azure.Messaging;
using Azure.Messaging.ServiceBus;
using Bridge.Bus;
using Microsoft.Extensions.Caching.Memory;

namespace Microsoft.Extensions.DependencyInjection;

internal class AzureServiceBusSender : IMessageBus
{
    private const string CloudEventsContentType = "application/cloudevents+json";
    
    private readonly ServiceBusClient _client;
    private readonly IMemoryCache _queueCache;

    public AzureServiceBusSender(
        ServiceBusClient client,
        IMemoryCache queueCache)
    {
        _client = client;
        _queueCache = queueCache;
    }

    public async ValueTask Send<TMessage>(TMessage message, string queue, CancellationToken cancellationToken)
    {
        var sender = CreateQueueSender(queue);

        var cloudEvent = new CloudEvent(_client.Identifier, typeof(TMessage).Name, message);

        ServiceBusMessage serviceBusMessage =
            new ServiceBusMessage(new BinaryData(cloudEvent)) { ContentType = CloudEventsContentType };

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
    }

    private ServiceBusSender CreateQueueSender(string queue)
    {
        return _queueCache
            .GetOrCreate(queue, entry => CreateSender(entry, queue))!;
    }

    private ServiceBusSender CreateSender(ICacheEntry entry, string queue)
    {
        entry.Size = 1;
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30);
        entry.SetPriority(CacheItemPriority.NeverRemove);
        entry.RegisterPostEvictionCallback((key, value, reason, state) =>
        {
            if (value is ServiceBusSender sender)
            {
                _ = sender.DisposeAsync();
            }
        });

        return _client.CreateSender(queue);
    }
}