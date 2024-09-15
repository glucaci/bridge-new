using Bridge.Bus;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static BusBridgeBuilder UsingAzureServiceBus(
        this BusBridgeBuilder builder,
        string connectionString)
    {
        return builder;
    }
}