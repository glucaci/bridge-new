using Microsoft.Extensions.DependencyInjection;

namespace Bridge;

public class BusBridgeBuilder
{
    private readonly List<Func<IServiceProvider, ConsumerConfiguration>> _consumers = new();

    internal BusBridgeBuilder(IServiceCollection services)
    {
        Services = services;
    }

    internal IReadOnlyList<Func<IServiceProvider, ConsumerConfiguration>> Consumers => _consumers;

    public IServiceCollection Services { get; }

    public BusBridgeBuilder AddConsumer<TConsumer, TMessage>(
        string queueName,
        Action<ConsumerConfiguration>? configure = default)
        where TConsumer : class, IConsumer<TMessage>
    {
        Services.AddSingleton<TConsumer>();

        _consumers.Add(sp =>
        {
            var serializer = sp.GetRequiredService<ISerializer>();
            
            var consumerConfiguration = ConsumerConfiguration
                .Create<TConsumer, TMessage>(queueName, serializer);

            configure?.Invoke(consumerConfiguration);

            return consumerConfiguration;
        });

        return this;
    }
    
    public BusBridgeBuilder AddOutbox(Action<BusBridgeOutboxBuilder> configure)
    {
        Services.AddHostedService<OutboxProcessor>();

        var builder = new BusBridgeOutboxBuilder(Services);
        configure(builder);

        return this;
    }
}
