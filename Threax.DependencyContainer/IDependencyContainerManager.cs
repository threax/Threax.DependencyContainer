using System.Diagnostics;

namespace Threax.DependencyContainer;

public interface IDependencyContainerManager
{
    ProcessStartInfo Command(params string[] arguments);
    ProcessStartInfo Command(IEnumerable<string> arguments);
    ProcessStartInfo Command(IEnumerable<string> arguments, Dictionary<string, string> envVars);
    string GetContainerPath(string path);
}
