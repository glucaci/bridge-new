namespace Bridge.Bus.InMemory;

internal interface IInMemoryMessageBus : IMessageBus
{
    ItemsAwareChannel<InMemoryMessage> GetQueue(string queue);
}