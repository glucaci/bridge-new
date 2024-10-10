using Bridge;
using Bridge.InMemory;

namespace Microsoft.Extensions.DependencyInjection;

internal static class BusBridgeOutboxBuilderExtensions
{
    public static BusBridgeOutboxBuilder UsingInMemory(
        this BusBridgeOutboxBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.AddSingleton<IOutboxStorage, InMemoryOutboxStorage>();

        return builder;
    }
}