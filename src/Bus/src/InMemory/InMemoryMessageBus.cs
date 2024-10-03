using Azure.Messaging;

namespace Bridge.Bus.InMemory;

internal class InMemoryMessageBus : IMessageBus
{
    private readonly TimeProvider _timeProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string,ConsumerConfiguration> _consumersMap;

    public InMemoryMessageBus(
        TimeProvider timeProvider,
        IServiceProvider serviceProvider,
        IReadOnlyList<ConsumerConfiguration> consumers)
    {
        _timeProvider = timeProvider;
        _serviceProvider = serviceProvider;
        _consumersMap = consumers.ToDictionary(c => c.QueueName, c => c);
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
        DateTimeOffset now = _timeProvider.GetUtcNow();
        DateTimeOffset enqueueTime = now;

        if (scheduledEnqueueTime.HasValue)
        {
            enqueueTime = scheduledEnqueueTime.Value;
        }

        if (now >= enqueueTime)
        {
            CloudEvent cloudEvent = new CloudEvent(
                nameof(InMemoryMessageBus), typeof(TMessage).Name, message);
            
            if (_consumersMap.TryGetValue(queue, out var consumerConfiguration))
            {
                await consumerConfiguration
                    .HandleMessage(_serviceProvider, cloudEvent, cancellationToken);
            }
        }
    }
}