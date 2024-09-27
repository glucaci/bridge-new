namespace Bridge.Workflow.Bus.AzureServiceBus;

internal class AzureServiceBusProvider : IQueueProvider
{
    public bool IsDequeueBlocking => false;

    public Task<string> DequeueWork(QueueType queue, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task QueueWork(string id, QueueType queue)
    {
        throw new NotImplementedException();
    }

    public Task Start()
    {
        throw new NotImplementedException();
    }

    public Task Stop()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
    }
}