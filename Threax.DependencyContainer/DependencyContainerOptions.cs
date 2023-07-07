namespace Threax.DependencyContainer;

public class DependencyContainerOptions
{
    /// <summary>
    /// The name of the image.
    /// </summary>
    public String ImageName { get; init; }

    /// <summary>
    /// The name of the container.
    /// </summary>
    public String ContainerName { get; init; }

    /// <summary>
    /// This will be true if the code is running in the dependency container.
    /// </summary>
    public bool InContainer { get; set; } = Environment.GetEnvironmentVariable("IAC_IN_CONTAINER") == "true";

    /// <summary>
    /// A dictionary of directories you want to mount into the dependency container. These are used by GetContainerPath to help
    /// retarget files.
    /// </summary>
    public Dictionary<string, string> DirectoryMounts = new Dictionary<string, string>();
}
