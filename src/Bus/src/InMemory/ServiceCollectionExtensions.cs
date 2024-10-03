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

        builder.Services.AddSingleton<IMessageBus>(sp => 
            new InMemoryMessageBus(timeProvider, sp, builder.Consumers));

        return builder;
    }
}