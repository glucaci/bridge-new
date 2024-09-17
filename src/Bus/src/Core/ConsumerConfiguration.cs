using Azure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Bus;

internal delegate ValueTask HandleMessageDelegate(
    IServiceProvider serviceProvider, 
    CloudEvent cloudEvent, 
    CancellationToken cancellationToken);

internal class ConsumerConfiguration
{
    public string QueueName { get; }
    public Type HandlerType { get; }
    public Type MessageType { get; }
    public HandleMessageDelegate HandleMessage { get; }

    private ConsumerConfiguration(
        string queueName,
        Type handlerType,
        Type messageType,
        HandleMessageDelegate handleMessage)
    {
        QueueName = queueName;
        HandlerType = handlerType;
        MessageType = messageType;
        HandleMessage = handleMessage;
    }

    private static T Convert<T>(CloudEvent cloudEvent)
    {
        return cloudEvent.Data!.ToObjectFromJson<T>();
    }

    public static ConsumerConfiguration Create<TConsumer, TMessage>(string queueName)
        where TConsumer : IConsumer<TMessage>
    {
        HandleMessageDelegate handleMessage =
            async (serviceProvider, cloudEvent, cancellationToken) =>
            {
                var handler = serviceProvider.GetRequiredService<TConsumer>();
                var message = Convert<TMessage>(cloudEvent);
                await handler.Handle(message, cancellationToken);
            };

        return new ConsumerConfiguration(queueName, typeof(TConsumer), typeof(TMessage), handleMessage);
    }
}