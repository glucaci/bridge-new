using Bridge;
using Bridge.InMemory;

namespace Microsoft.Extensions.DependencyInjection;

internal static class BusBridgeBuilderExtensions
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

        foreach (var consumerConfiguration in builder.Consumers)
        {
            builder.Services.AddHostedService(sp =>
                new InMemoryProcessor(sp, consumerConfiguration(sp), timeProvider));
        }

        builder.Services.AddSingleton<IInMemoryMessageBus>(_ => new InMemoryMessageBus(timeProvider));
        builder.Services.AddSingleton<IMessageBus>(sp => sp.GetRequiredService<IInMemoryMessageBus>());

        return builder;
    }
}