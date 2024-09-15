using Bridge.Bus;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static BusBridgeBuilder AddBridgeBus(
        this IServiceCollection services)
    {
        return new BusBridgeBuilder(services);
    }
}