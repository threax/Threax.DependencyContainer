using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Threax.DependencyContainer.Tests;

static class TestServices
{
    public static void AddTestServices(this IServiceCollection services)
    {
        services.AddLogging(o =>
        {
            o.AddConsole().AddSimpleConsole(co =>
            {
                co.IncludeScopes = false;
                co.SingleLine = true;
            });
        });

        services.AddThreaxProcessHelper();
    }
}
