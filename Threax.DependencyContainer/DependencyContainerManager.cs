using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Threax.ProcessHelper;

namespace Threax.DependencyContainer;

class DependencyContainerManager : IDisposable, IDependencyContainerManager
{
    private Dictionary<string, string> paths = new Dictionary<string, string>();
    private readonly ILogger<DependencyContainerManager> logger;
    private readonly IProcessRunner processRunner;
    private readonly DependencyContainerOptions options;
    private bool startedContainer = false;

    public DependencyContainerManager(ILogger<DependencyContainerManager> logger, IProcessRunner processRunner, DependencyContainerOptions options)
    {
        this.logger = logger;
        this.processRunner = processRunner;
        this.options = options;

        logger.LogInformation($"Starting dependency container '{options.ContainerName}'...");

        var startInfo = new ProcessStartInfo("docker")
        {
            ArgumentList =
            {
                "run", "--rm", "-d",
                "--name", options.ContainerName,
            }
        };

        foreach (var mount in options.DirectoryMounts)
        {
            paths.Add(NormalizeInputPath(mount.Key), NormalizeInputPath(mount.Value));

            startInfo.ArgumentList.Add("-v");
            startInfo.ArgumentList.Add($"{mount.Key}:{mount.Value}");
        }

        startInfo.ArgumentList.Add(options.ImageName);
        startInfo.ArgumentList.Add("/bin/bash");
        startInfo.ArgumentList.Add("-c");
        startInfo.ArgumentList.Add("tail -f /dev/null");

        processRunner.RunVoid(startInfo, $"Error starting dependency container '{options.ContainerName}'.");

        startedContainer = true;
    }

    public void Dispose()
    {
        if (startedContainer)
        {
            logger.LogInformation($"Closing dependency container '{options.ContainerName}'...");

            ProcessStartInfo startInfo = new ProcessStartInfo("docker")
            {
                ArgumentList =
                {
                    "kill",
                    options.ContainerName,
                }
            };

            var killProcess = Process.Start(startInfo);
            killProcess.WaitForExit();

            startedContainer = false;

            logger.LogInformation($"Dependency container '{options.ContainerName}' closed.");
        }
    }

    public ProcessStartInfo Command(params string[] arguments)
    {
        return Command((IEnumerable<String>)arguments);
    }

    public ProcessStartInfo Command(IEnumerable<String> arguments)
    {
        return Command(arguments, null);
    }

    public ProcessStartInfo Command(IEnumerable<String> arguments, Dictionary<string, string> envVars)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("docker")
        {
            ArgumentList =
            {
                "exec", "-it",
            }
        };

        if (envVars != null)
        {
            foreach (var envVar in envVars)
            {
                startInfo.ArgumentList.Add("-e");
                startInfo.ArgumentList.Add($"{envVar.Key}={envVar.Value}");
            }
        }

        startInfo.ArgumentList.Add(options.ContainerName);

        foreach (var arg in arguments)
        {
            startInfo.ArgumentList.Add(arg);
        }

        return startInfo;
    }

    public String GetContainerPath(String path)
    {
        var normalizedPath = NormalizePath(path);
        string basePath = null;
        foreach (var test in paths)
        {
            if (normalizedPath.StartsWith(test.Key) || test.Key == normalizedPath + "/")
            {
                if (basePath == null || test.Key.Length > basePath.Length)
                {
                    basePath = test.Key;
                }
            }
        }

        if (basePath == null)
        {
            throw new InvalidOperationException($"Cannot find path '{path}' in any mounted container paths.");
        }

        var newPath = paths[basePath];
        if (basePath == normalizedPath + "/") //The base path needed the / to match the input path, so remove this part
        {
            return newPath.Substring(0, newPath.Length - 1);
        }

        //Otherwise just combine the 2 paths together
        return newPath + normalizedPath.Substring(basePath.Length);
    }

    private string NormalizePath(String path)
    {
        return path.Replace("\\", "/");
    }

    private string NormalizeInputPath(String path)
    {
        path = NormalizePath(path);

        if (!path.EndsWith("/"))
        {
            path += "/";
        }

        return path;
    }
}
