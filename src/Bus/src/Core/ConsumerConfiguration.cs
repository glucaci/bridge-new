﻿using Azure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Bus;

internal delegate ValueTask HandleMessageDelegate(
    IServiceProvider serviceProvider,
    CloudEvent cloudEvent,
    CancellationToken cancellationToken);

public class ConsumerConfiguration
{
    internal string QueueName { get; }
    internal Type HandlerType { get; }
    internal Type MessageType { get; }
    internal HandleMessageDelegate HandleMessage { get; }

    private TimeSpan _maxProcessingTime = TimeSpan.FromMinutes(5);
    public TimeSpan MaxProcessingTime
    {
        get => _maxProcessingTime;
        set => _maxProcessingTime = TimeSpan.FromMinutes(Math.Max(5, value.TotalMinutes));
    }

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

    internal static ConsumerConfiguration Create<TConsumer, TMessage>(string queueName)
        where TConsumer : IConsumer<TMessage>
    {
        HandleMessageDelegate handleMessage =
            async (serviceProvider, cloudEvent, cancellationToken) =>
            {
                TConsumer handler = serviceProvider.GetRequiredService<TConsumer>();
                TMessage message = Convert<TMessage>(cloudEvent);
                await handler.Handle(message, cancellationToken);
            };

        return new ConsumerConfiguration(queueName, typeof(TConsumer), typeof(TMessage), handleMessage);
    }
}
