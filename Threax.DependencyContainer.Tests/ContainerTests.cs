using Microsoft.Extensions.DependencyInjection;
using Threax.Extensions.DependencyInjection;
using Threax.ProcessHelper;

namespace Threax.DependencyContainer.Tests;

[TestClass]
public class ContainerTests
{
    [TestMethod]
    public void BasicStart()
    {
        var services = new ServiceCollection();
        services.AddDependencyContainer("mcr.microsoft.com/dotnet/sdk:6.0", "containertests-basicstart");
        services.AddTestServices();
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var depContainer = scope.ServiceProvider.GetRequiredService<IDependencyContainerManager>();
    }

    [TestMethod]
    public void RunCommand()
    {
        var services = new ServiceCollection();
        services.AddDependencyContainer("mcr.microsoft.com/dotnet/sdk:6.0", "containertests-runcommand");
        services.AddTestServices();
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var depContainer = scope.ServiceProvider.GetRequiredService<IDependencyContainerManager>();
        var processRunner = scope.ServiceProvider.GetRequiredService<IProcessRunner>();

        var startInfo = depContainer.Command("echo", "hi");
        var result = processRunner.RunStringProcess(startInfo, "Error echoing hi");
        Assert.AreEqual("hi", result);
    }

    [TestMethod]
    public void RunMultipleCommands()
    {
        var services = new ServiceCollection();
        services.AddDependencyContainer("mcr.microsoft.com/dotnet/sdk:6.0", "containertests-runmultiplecommands");
        services.AddTestServices();
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var depContainer = scope.ServiceProvider.GetRequiredService<IDependencyContainerManager>();
        var processRunner = scope.ServiceProvider.GetRequiredService<IProcessRunner>();

        var startInfo = depContainer.Command("echo", "hi");
        var result = processRunner.RunStringProcess(startInfo, "Error echoing hi");
        Assert.AreEqual("hi", result);

        startInfo = depContainer.Command("echo", "bye");
        result = processRunner.RunStringProcess(startInfo, "Error echoing bye");
        Assert.AreEqual("bye", result);
    }

    [TestMethod]
    public void TestMountNoSlash()
    {
        var workingDir = Directory.GetCurrentDirectory();
        var mountPath = "/mounted/workingdir";

        var services = new ServiceCollection();
        services.AddDependencyContainer("mcr.microsoft.com/dotnet/sdk:6.0", "containertests-testmountnoslash", (s, o) =>
        {
            o.DirectoryMounts = new Dictionary<string, string>
                {
                    { workingDir, mountPath }
                };
        });
        services.AddTestServices();
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var depContainer = scope.ServiceProvider.GetRequiredService<IDependencyContainerManager>();
        var processRunner = scope.ServiceProvider.GetRequiredService<IProcessRunner>();

        var inContainerPath = depContainer.GetContainerPath(workingDir);

        Assert.AreEqual(mountPath, inContainerPath);
    }

    [TestMethod]
    public void TestMountWithSlash()
    {
        var workingDir = Directory.GetCurrentDirectory() + "\\";
        var mountPath = "/mounted/workingdir";

        var services = new ServiceCollection();
        services.AddDependencyContainer("mcr.microsoft.com/dotnet/sdk:6.0", "containertests-testmountwithslash", (s, o) =>
        {
            o.DirectoryMounts = new Dictionary<string, string>
                {
                    { workingDir, mountPath }
                };
        });
        services.AddTestServices();
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var depContainer = scope.ServiceProvider.GetRequiredService<IDependencyContainerManager>();
        var processRunner = scope.ServiceProvider.GetRequiredService<IProcessRunner>();

        var inContainerPath = depContainer.GetContainerPath(workingDir);

        Assert.AreEqual("/mounted/workingdir/", inContainerPath);
    }

    [TestMethod]
    public void TestMountWithPath()
    {
        var workingDir = Directory.GetCurrentDirectory();
        var mountPath = "/mounted/workingdir";

        var services = new ServiceCollection();
        services.AddDependencyContainer("mcr.microsoft.com/dotnet/sdk:6.0", "containertests-testmountwithpath", (s, o) =>
        {
            o.DirectoryMounts = new Dictionary<string, string>
            {
                { workingDir, mountPath }
            };
        });
        services.AddTestServices();
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var depContainer = scope.ServiceProvider.GetRequiredService<IDependencyContainerManager>();
        var processRunner = scope.ServiceProvider.GetRequiredService<IProcessRunner>();

        var inContainerPath = depContainer.GetContainerPath(workingDir + "\\More\\Test\\Dirs");

        Assert.AreEqual("/mounted/workingdir/More/Test/Dirs", inContainerPath);
    }
}