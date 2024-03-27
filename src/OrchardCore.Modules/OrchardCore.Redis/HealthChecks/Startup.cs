using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace OrchardCore.Redis.HealthChecks;

[RequireFeatures("OrchardCore.HealthChecks")]
public class Startup : StartupBase
{
    // The order of this startup configuration should run early to register the redis check.
    public override int Order => 100;

    public override void ConfigureServices(IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddRedisCheck();
    }
}
