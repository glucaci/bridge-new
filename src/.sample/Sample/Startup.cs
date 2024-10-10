using Bridge;

namespace Bridge.Sample;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();

        BridgeOptions options = _configuration.GetSection("Bridge").Get<BridgeOptions>()!;

        services
            .AddBridge()
            .AddConsumer<DocumentCreatedHandler, DocumentCreated>(options.QueueName)
            .AddOutbox(o =>
            {
                o.
            })
            .UsingAzureServiceBus(o =>
            {
                o.ConnectionString = options.BusConnectionString;
            });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(builder =>
        {
            builder.MapGet("/", () => DateTimeOffset.Now);
        });
    }
}

public record BridgeOptions(string BusConnectionString, string QueueName);

public record DocumentCreated(string Id);

public class DocumentCreatedHandler : IConsumer<DocumentCreated>
{
    public ValueTask Handle(DocumentCreated message, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}