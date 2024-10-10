using System.Diagnostics;
using System.Transactions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bridge;

internal class OutboxProcessor : BackgroundService
{
    private readonly IOutboxStorage _outboxStorage;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        IOutboxStorage outboxStorage,
        IMessageBus messageBus,
        ILogger<OutboxProcessor> logger)
    {
        _outboxStorage = outboxStorage;
        _messageBus = messageBus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Activity? activity = BridgeBusActivity.StartProcessOutbox();
                Activity? processActivity = default;
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    OutboxItem? outboxItem = await _outboxStorage
                        .Delete(stoppingToken);

                    if (outboxItem is not null)
                    {
                        activity?.Dispose();
                        processActivity = BridgeBusActivity.StartProcessOutbox(outboxItem);

                        await _messageBus
                            .Send(outboxItem.Message, outboxItem.Queue, stoppingToken);

                        // TODO: Optional if archive is enabled
                        OutboxItem sentOutboxItem = SentOutboxItem
                            .FromOutboxItem(outboxItem, DateTime.UtcNow);

                        await _outboxStorage.Archive(sentOutboxItem, stoppingToken);
                        // TODO: ----------------------------

                        scope.Complete();
                    }
                }

                processActivity?.Dispose();
                activity?.Dispose();

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed processing outbox");
            }
        }
    }
}