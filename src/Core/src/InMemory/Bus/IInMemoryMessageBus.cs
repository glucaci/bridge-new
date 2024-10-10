namespace Bridge.InMemory;

internal interface IInMemoryMessageBus : IMessageBus
{
    InMemoryQueue<InMemoryMessage> GetQueue(string queue);
}