using System.Diagnostics;

namespace Threax.DependencyContainer;

public interface IDependencyContainerManager
{
    /// <summary>
    /// Return a ProcessStartInfo configured to run in the managed container.
    /// </summary>
    ProcessStartInfo Command(params string[] arguments);

    /// <summary>
    /// Return a ProcessStartInfo configured to run in the managed container.
    /// </summary>
    ProcessStartInfo Command(IEnumerable<string> arguments);

    /// <summary>
    /// Return a ProcessStartInfo configured to run in the managed container. The environment variables passed will also be set.
    /// </summary>
    ProcessStartInfo Command(IEnumerable<string> arguments, Dictionary<string, string> envVars);

    /// <summary>
    /// Get the path inside the target container for the input path. Will throw an InvalidOperationException if the path is not mounted.
    /// </summary>
    string GetContainerPath(string path);
}
