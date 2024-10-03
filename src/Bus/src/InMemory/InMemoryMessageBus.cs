using System.Collections.Concurrent;
using System.Threading.Channels;
using Azure.Messaging;

namespace Bridge.Bus.InMemory;

internal class InMemoryMessageBus : IInMemoryMessageBus
{
    private readonly ConcurrentDictionary<string, ItemsAwareChannel<InMemoryMessage>> _queues = new();
    private readonly TimeProvider _timeProvider;

    public InMemoryMessageBus(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public ValueTask Send<TMessage>(
        TMessage message,
        string queue,
        CancellationToken cancellationToken)
    {
        return SendMessage(message, queue, cancellationToken);
    }

    public ValueTask Schedule<TMessage>(
        TMessage message,
        string queue,
        DateTimeOffset enqueueTime,
        CancellationToken cancellationToken)
    {
        return SendMessage(message, queue, cancellationToken, enqueueTime);
    }

    private async ValueTask SendMessage<TMessage>(
        TMessage message,
        string queue,
        CancellationToken cancellationToken,
        DateTimeOffset? scheduledEnqueueTime = default)
    {
        var queueInstance = GetQueue(queue);

        DateTimeOffset enqueueTime = _timeProvider.GetUtcNow();

        if (scheduledEnqueueTime.HasValue)
        {
            enqueueTime = scheduledEnqueueTime.Value;
        }

        CloudEvent cloudEvent = new CloudEvent(
            nameof(InMemoryMessageBus), typeof(TMessage).Name, message);

        var inMemoryMessage = new InMemoryMessage(enqueueTime, cloudEvent);

        await queueInstance.Enqueue(inMemoryMessage, cancellationToken);
    }

    public ItemsAwareChannel<InMemoryMessage> GetQueue(string queue)
    {
        return _queues.GetOrAdd(queue, _ =>
            new ItemsAwareChannel<InMemoryMessage>(Channel.CreateBounded<InMemoryMessage>(
                new BoundedChannelOptions(100) { FullMode = BoundedChannelFullMode.Wait })));
    }
}