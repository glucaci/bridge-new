using System.Collections.Concurrent;

namespace Bridge.InMemory;

internal class InMemoryOutboxStorage : IOutboxStorage
{
    private readonly ConcurrentQueue<OutboxItem> _pendingItems = new();
    private readonly ConcurrentDictionary<string, OutboxItem> _archivedItems = new();
    
    public ValueTask Add(OutboxItem outboxItem, CancellationToken cancellationToken)
    {
        _pendingItems.Enqueue(outboxItem);
        return ValueTask.CompletedTask;
    }
    
    public ValueTask<OutboxItem?> Delete(CancellationToken cancellationToken)
    {
        if (_pendingItems.TryDequeue(out var outboxItem))
        {
            return ValueTask.FromResult<OutboxItem?>(outboxItem);
        }

        return ValueTask.FromResult<OutboxItem?>(null);
    }

    public ValueTask Archive(OutboxItem outboxItem, CancellationToken cancellationToken)
    {
        _archivedItems[outboxItem.Id] = outboxItem;
        return ValueTask.CompletedTask;
    }
}