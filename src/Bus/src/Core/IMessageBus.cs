namespace Bridge.Bus;

public interface IMessageBus
{
    ValueTask Send<TMessage>(TMessage message, string queue, CancellationToken cancellationToken);
}