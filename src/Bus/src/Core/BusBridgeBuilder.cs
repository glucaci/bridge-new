using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Bus;

public class BusBridgeBuilder
{
    private readonly List<ConsumerConfiguration> _consumers = new();

    internal BusBridgeBuilder(IServiceCollection services)
    {
        Services = services;
    }

    internal IReadOnlyList<ConsumerConfiguration> Consumers => _consumers;

    public IServiceCollection Services { get; }

    public BusBridgeBuilder AddConsumer<TConsumer, TMessage>(
        string queueName,
        Action<ConsumerConfiguration>? configure = default)
        where TConsumer : class, IConsumer<TMessage>
    {
        Services.AddSingleton<TConsumer>();
        var consumerConfiguration = ConsumerConfiguration.Create<TConsumer, TMessage>(queueName);

        configure?.Invoke(consumerConfiguration);

        _consumers.Add(consumerConfiguration);

        return this;
    }
}
