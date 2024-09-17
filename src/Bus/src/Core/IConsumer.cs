namespace Bridge.Bus;

public interface IConsumer<in TMessage>
{
    ValueTask Handle(TMessage message, CancellationToken cancellationToken);
}