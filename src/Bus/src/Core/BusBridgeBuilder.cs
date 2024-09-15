using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Bus;

public class BusBridgeBuilder
{
    public IServiceCollection Services { get; }

    public BusBridgeBuilder(IServiceCollection services)
    {
        Services = services;
    }
}