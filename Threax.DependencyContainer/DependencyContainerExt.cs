using Microsoft.Extensions.DependencyInjection;
using Threax.DependencyContainer;

namespace Threax.Extensions.DependencyInjection;

public static class DependencyContainerExt
{
    public static void AddDependencyContainer(this IServiceCollection services, String imageName, String containerName, Action<IServiceProvider, DependencyContainerOptions> configure = null)
    {
        services.AddSingleton<DependencyContainerOptions>(s =>
        {
            var options = new DependencyContainerOptions()
            {
                ImageName = imageName,
                ContainerName = containerName,
            };
            configure?.Invoke(s, options);
            return options;
        });
        services.AddSingleton<DependencyContainerManager>();
        services.AddSingleton<NoDependencyContainerManager>();
        services.AddSingleton<IDependencyContainerManager>(s =>
        {
            var options = s.GetRequiredService<DependencyContainerOptions>();
            if (options.InContainer)
            {
                return s.GetRequiredService<NoDependencyContainerManager>();
            }
            else
            {
                return s.GetRequiredService<DependencyContainerManager>();
            }
        });
    }
}
