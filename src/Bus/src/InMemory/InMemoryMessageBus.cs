using System.Collections.Concurrent;
using System.Threading.Channels;
using Azure.Messaging;

namespace Bridge.Bus.InMemory;

internal record InMemoryMessage(DateTimeOffset EnqueueTime, CloudEvent CloudEvent);

internal class InMemoryMessageBus : IInMemoryMessageBus
{
    private readonly ConcurrentDictionary<string, Channel<InMemoryMessage>> _queues = new();
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

    private ValueTask SendMessage<TMessage>(
        TMessage message,
        string queue,
        CancellationToken cancellationToken,
        DateTimeOffset? scheduledEnqueueTime = default)
    {
        var channel = GetOrCreateChannel(queue);

        DateTimeOffset enqueueTime = _timeProvider.GetUtcNow();

        if (scheduledEnqueueTime.HasValue)
        {
            enqueueTime = scheduledEnqueueTime.Value;
        }

        CloudEvent cloudEvent = new CloudEvent(
            nameof(InMemoryMessageBus), typeof(TMessage).Name, message);

        var inMemoryMessage = new InMemoryMessage(enqueueTime, cloudEvent);

        return channel.Writer.WriteAsync(inMemoryMessage, cancellationToken);
    }

    private Channel<InMemoryMessage> GetOrCreateChannel(string queue)
    {
        return _queues.GetOrAdd(queue, _ =>
            Channel.CreateBounded<InMemoryMessage>(
                new BoundedChannelOptions(100) { FullMode = BoundedChannelFullMode.Wait }));
    }

    public Channel<InMemoryMessage> GetChannelFor(string queue)
    {
        return _queues.TryGetValue(queue, out var channel)
            ? channel
            : throw new InvalidOperationException($"Queue '{queue}' does not exist.");
    }
}