using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Bus;

public class BusBridgeBuilder
{
    private readonly List<ConsumerConfiguration> _consumers = new();

    public BusBridgeBuilder(IServiceCollection services)
    {
        Services = services;
    }
    
    internal IReadOnlyList<ConsumerConfiguration> Consumers => _consumers;

    public IServiceCollection Services { get; }

    public BusBridgeBuilder AddConsumer<TConsumer, TMessage>(string queueName)
        where TConsumer : class, IConsumer<TMessage>
    {
        Services.AddSingleton<TConsumer>();
        _consumers.Add(ConsumerConfiguration.Create<TConsumer, TMessage>(queueName));

        return this;
    }
}