using Bridge;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static BusBridgeBuilder AddBridge(
        this IServiceCollection services)
    {
        return new BusBridgeBuilder(services);
    }
}