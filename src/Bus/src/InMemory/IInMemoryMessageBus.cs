using System.Threading.Channels;

namespace Bridge.Bus.InMemory;

internal interface IInMemoryMessageBus : IMessageBus
{
    Channel<InMemoryMessage> GetChannelFor(string queue);
}