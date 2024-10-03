using Bridge.Bus;
using Bridge.Bus.InMemory;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static BusBridgeBuilder UsingInMemory(this BusBridgeBuilder builder) =>
        UsingInMemory(builder, TimeProvider.System);

    public static BusBridgeBuilder UsingInMemory(
        this BusBridgeBuilder builder,
        TimeProvider timeProvider)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        foreach (ConsumerConfiguration consumerConfiguration in builder.Consumers)
        {
            builder.Services.AddHostedService(sp =>
                new InMemoryProcessor(sp, consumerConfiguration, timeProvider));
        }

        builder.Services.AddSingleton<IInMemoryMessageBus>(_ => new InMemoryMessageBus(timeProvider));
        builder.Services.AddSingleton<IMessageBus>(sp => sp.GetRequiredService<IInMemoryMessageBus>());

        return builder;
    }
}