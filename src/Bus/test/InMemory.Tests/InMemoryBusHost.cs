using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace Bridge.Bus.InMemory.Tests;

public class InMemoryBusHost<TConsumer, TMessage> : IAsyncLifetime 
    where TConsumer : class, IConsumer<TMessage>
{
    private readonly IServiceCollection _services;
    private IServiceProvider _serviceProvider;

    public InMemoryBusHost()
    {
        TimeProvider = new FakeTimeProvider();
        QueueName = Guid.NewGuid().ToString("N");
        
        _services = new ServiceCollection();
        _services
            .AddBridgeBus()
            .AddConsumer<TConsumer, TMessage>(QueueName)
            .UsingInMemory(TimeProvider);
    }

    public string QueueName { get; }
    public FakeTimeProvider TimeProvider { get; }
    public IMessageBus MessageBus { get; private set; }
    public TConsumer Consumer { get; private set; }
    
    public async Task InitializeAsync()
    {
        _serviceProvider = _services.BuildServiceProvider();
        
        var hostedServices = _serviceProvider.GetServices<IHostedService>();
        foreach (var service in hostedServices)
        {
            await service.StartAsync(default);
        }
        
        MessageBus = _serviceProvider.GetRequiredService<IMessageBus>();
        Consumer = _serviceProvider.GetRequiredService<TConsumer>();
    }

    public async Task DisposeAsync()
    {
        var hostedServices = _serviceProvider.GetServices<IHostedService>();
        foreach (var service in hostedServices)
        {
            await service.StopAsync(default);
        }
    }
}