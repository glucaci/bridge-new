using System.Collections.Concurrent;

namespace Bridge.Bus;

internal class InMemoryMessageBus : IMessageBus
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<object>> _queues = new();

    public ValueTask Send<TMessage>(
        TMessage message,
        string queue,
        CancellationToken cancellationToken)
    {
        if (_queues.TryGetValue(queue, out ConcurrentQueue<object>? messages))
        {
            messages.Enqueue(message);
        }
        else
        {
            _queues.TryAdd(queue, new ConcurrentQueue<object>(new object[] { message }));
        }


        return ValueTask.CompletedTask;
    }

    public ValueTask Schedule<TMessage>(
        TMessage message,
        string queue,
        DateTimeOffset enqueueTime,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled(cancellationToken);
        }

        TimeSpan delay = enqueueTime - DateTimeOffset.Now;

        if (delay <= TimeSpan.Zero)
        {
            return Send(message, queue, cancellationToken);
        }

        _ = ScheduleMessageAsync(message, queue, delay);

        return ValueTask.CompletedTask;
    }

    private async Task ScheduleMessageAsync<TMessage>(
        TMessage message,
        string queue,
        TimeSpan delay)
    {
        try
        {
            await Task.Delay(delay);
            await Send(message, queue, CancellationToken.None);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}