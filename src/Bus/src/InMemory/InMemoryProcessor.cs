using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bridge.Bus.InMemory;

public class InMemoryProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerConfiguration _consumerConfiguration;
    private readonly TimeProvider _timeProvider;
    private readonly IInMemoryMessageBus _messageBus;

    public InMemoryProcessor(
        IServiceProvider serviceProvider,
        ConsumerConfiguration consumerConfiguration, 
        TimeProvider timeProvider)
    {
        _serviceProvider = serviceProvider;
        _consumerConfiguration = consumerConfiguration;
        _timeProvider = timeProvider;
        _messageBus = _serviceProvider.GetRequiredService<IInMemoryMessageBus>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var channel = _messageBus.GetChannelFor(_consumerConfiguration.QueueName);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var (enqueueTime, cloudEvent) = await channel.Reader.ReadAsync(stoppingToken);
                var now = _timeProvider.GetUtcNow();

                if (now >= enqueueTime)
                {
                    await _consumerConfiguration.HandleMessage(_serviceProvider, cloudEvent, stoppingToken);
                }
                else
                {
                    var inMemoryMessage = new InMemoryMessage(enqueueTime, cloudEvent);
                    await channel.Writer.WriteAsync(inMemoryMessage, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}