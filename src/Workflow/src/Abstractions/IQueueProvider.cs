namespace Bridge.Workflow;

public interface IQueueProvider : IDisposable
{
    Task QueueWork(string id, QueueType queue);

    Task<string> DequeueWork(QueueType queue, CancellationToken cancellationToken);

    bool IsDequeueBlocking { get; }

    Task Start();

    Task Stop();
}

public enum QueueType { Workflow = 0, Event = 1, Index = 2 }