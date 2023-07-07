using System.Diagnostics;

namespace Threax.DependencyContainer;

class NoDependencyContainerManager : IDependencyContainerManager
{
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
        ProcessStartInfo startInfo = new ProcessStartInfo(arguments.First());

        if (envVars != null)
        {
            foreach (var envVar in envVars)
            {
                startInfo.Environment[envVar.Key] = envVar.Value;
            }
        }

        foreach (var arg in arguments.Skip(1))
        {
            startInfo.ArgumentList.Add(arg);
        }

        return startInfo;
    }

    public string GetContainerPath(string path)
    {
        return NormalizePath(path);
    }

    private string NormalizePath(String path)
    {
        return path.Replace("\\", "/");
    }
}
