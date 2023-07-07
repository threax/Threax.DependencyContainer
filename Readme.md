# Threax.DependencyContainer
This library helps to manage a container that contains binaries you want to call from another program while controlling the versions of those binaries.

## Register in services
Register in the DI container by using:
```
services.AddDependencyContainer("localhost/iac-container", "iac-container");
```

The first argument is the name of the image to use and the second is the name of the container when it is running.

## Mounting directories
You can also mount directories when registering using:
```
services.AddDependencyContainer("localhost/iac-container", "iac-container", (s, o) =>
{
    o.DirectoryMounts = new Dictionary<string, string>
    {
        { "/my/local/path", "/my/container/path" }
    };
});
```

You can then get this path by injecting IDependencyContainerManager and calling `GetContainerPath` with the local path to the file. This will be converted to the in container path you can pass to your command.