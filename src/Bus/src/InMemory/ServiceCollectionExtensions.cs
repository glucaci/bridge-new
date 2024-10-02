using Bridge.Bus;
using Bridge.Bus.InMemory;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
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

        builder.Services.AddSingleton<IMessageBus>(_ => new InMemoryMessageBus(timeProvider));

        return builder;
    }
}