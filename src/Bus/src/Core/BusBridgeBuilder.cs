using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bridge.Bus;

public class BusBridgeBuilder
{
    private readonly List<Consumer> _consumers = new();

    public BusBridgeBuilder(IServiceCollection services)
    {
        Services = services;
    }
    
    internal IReadOnlyList<Consumer> Consumers => _consumers;

    public IServiceCollection Services { get; }

    public BusBridgeBuilder AddConsumer<TConsumer, TMessage>(string queueName)
        where TConsumer : class, IConsumer<TMessage>
    {
        Services.AddSingleton<TConsumer>();
        var consumer = Consumer.Create<TConsumer, TMessage>(queueName);
        _consumers.Add(consumer);
        Services.AddSingleton(consumer);

        return this;
    }

    public void RegisterBrokerMessageHandler<T>(Func<IServiceProvider, T> brokerMessageHandlerFactory)
        where T : class, IBrokerMessageHandler
    {
        Services.AddHostedService(brokerMessageHandlerFactory);
    }
}

public interface IBrokerMessageHandler : IHostedService
{
}