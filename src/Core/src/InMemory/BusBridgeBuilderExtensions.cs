using Bridge;
using Bridge.InMemory;

namespace Microsoft.Extensions.DependencyInjection;

public static class BusBridgeBuilderExtensions
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

        foreach (var consumer in builder.Consumers)
        {
            builder.Services.AddHostedService(sp =>
                new InMemoryProcessor(sp, consumer.Create(sp), timeProvider));
        }

        builder.Services.AddSingleton<IInMemoryMessageBus>(_ => new InMemoryMessageBus(timeProvider));
        builder.Services.AddSingleton<IMessageBus>(sp => sp.GetRequiredService<IInMemoryMessageBus>());

        return builder;
    }
}