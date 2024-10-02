using Bridge.Bus;
using Bridge.Bus.InMemory;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static BusBridgeBuilder UsingInMemory(
        this BusBridgeBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.AddSingleton<IMessageBus, InMemoryMessageBus>();

        return builder;
    }
}