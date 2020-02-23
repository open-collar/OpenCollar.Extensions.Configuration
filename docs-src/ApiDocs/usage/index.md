# Basics

The purpose of the libarary is to provide a strongly typed, validated
wrapper around the values exposed as key/value pairs by configuration
providers in the 
[Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1)
library.

The starting point is to define an interface through which to read your 
configuration.  The interface must derive from
[IConfigurationObject](/api/OpenCollar.Extensions.Configuration.IConfigurationObject.md).
The interfaces must be public.  For example:

```cs
public interface IEnvironment : IConfigurationObject
{
    public string EnvironmentName { get; }

    public string Version { get; }
}
public interface IMyConfig : IConfigurationObject
{
    public IEnvironment Environment { get; }

    public string ReadOnlyString { get; }

    public string ReadWriteString { get; }
}
```

The next step is to register the interface as a service in
[`Startup.cs`](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-3.1).
At the same time the
[IConfigurationRoot](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationroot?view=dotnet-plat-ext-3.1)
object for the application must also be registered as a service.

```cs
public class Startup
{
    private readonly IConfigurationRoot _configuration;

    public Startup(IConfiguration configuration)
    {
        // Capture the configuration object passed in when the application is started.
        _configuration = (IConfigurationRoot)configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddSingleton(_configuration);
        services.AddConfigurationReader<IMyConfig>();
    }
    ...
}
```

Later, when needed, the configuration reader is available as a service:

```cs
public MyConstructor(IMyConfig config)
{
    var version = config.Environment.Version;
}
```

## Events

Any object returned implements the
[INotifyPropertyChanged](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=netstandard-2.1)
interface, allowing for changes to properties (whether from the underlying
configuration or from property changes made by code) to be detected.

Similary collections and dictionaries implement the
[INotifyCollectionChanged](https://docs.microsoft.com/en-us/dotnet/api/system.collections.specialized.inotifycollectionchanged?view=netstandard-2.1)
interface, similarly allowing changes to be detected, regardless of origin.

See "[Events](/usage/events.md)" for a more detailed review of the events and when they are raised.

## Fine Contol

Fine control over properties is provided by three attributes:

 * [`ConfigurationAttribute`](https://open-collar.github.io/OpenCollar.Extensions.Configuration/api/OpenCollar.Extensions.Configuration.ConfigurationAttribute.html) -
   basic control over default value, loading and saving.
 * [`PathAttribute`](https://open-collar.github.io/OpenCollar.Extensions.Configuration/api/OpenCollar.Extensions.Configuration.PathAttribute.html) -
   the naming and path to the underlying configuration key.

The "[Fine Control](/usage/control.md)" article looks in more depth at how attributes can be used to 
control the way in which configuration data is used.