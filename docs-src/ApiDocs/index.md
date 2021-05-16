# OpenCollar.Extensions.Configuration

Support for automatic validation, update and strongly-typed access to configuration.

## NuGet Package

Package and installation instructions at: https://www.nuget.org/packages/OpenCollar.Extensions.Configuration/

## Project
<table style="border-style: none; width: 100%;">
    <tr style="border-style: none;">
        <td style="width: 20%; border-style: none;">Latest Build:</td>
        <td style="width: 20%; border-style: none;"><a href="https://github.com/open-collar/OpenCollar.Extensions.Configuration/actions"><img src="https://img.shields.io/github/workflow/status/open-collar/OpenCollar.Extensions.Configuration/Build and Deploy"/></a></td>
        <td style="width: 20%; border-style: none;"><a href="https://coveralls.io/github/open-collar/OpenCollar.Extensions.Configuration?branch=master"><img src="https://coveralls.io/repos/github/open-collar/OpenCollar.Extensions.Configuration/badge.svg?branch=master"/></a></td>
        <td style="width: 20%; border-style: none;"><a href="https://www.nuget.org/packages/OpenCollar.Extensions.Configuration/"><img src="https://img.shields.io/nuget/vpre/OpenCollar.Extensions.Configuration?color=green"/></a></td>
        <td style="width: 20%; border-style: none;"><a href="https://www.nuget.org/packages/OpenCollar.Extensions.Configuration/"><img src="https://img.shields.io/nuget/dt/OpenCollar.Extensions.Configuration?color=green"/></a></td>
    </tr>
</table>

 * [Source Code on GitHub](https://github.com/open-collar/OpenCollar.Extensions.Configuration)
 * [Issue Tracking on GitHub](https://github.com/open-collar/OpenCollar.Extensions.Configuration/issues)
 * [Documentation on GitHub Pages](https://open-collar.github.io/OpenCollar.Extensions.Configuration/)

# Usage

The primary purpose of the libarary is to provide a strongly typed, validated
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

Later, when needed the configuration reader will be available as a service:

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

## Attributes

Fine control over properties is provided by three attributes:

 * [`ConfigurationAttribute`](https://open-collar.github.io/OpenCollar.Extensions.Configuration/api/OpenCollar.Extensions.Configuration.ConfigurationAttribute.html) -
   basic control over default value, loading and saving.
 * [`PathAttribute`](https://open-collar.github.io/OpenCollar.Extensions.Configuration/api/OpenCollar.Extensions.Configuration.PathAttribute.html) -
   the naming and path to the underlying configuration key.

## Validation

Add a validator to the services collection using the
[`AddConfigurationObjectValidator`](https://open-collar.github.io/OpenCollar.Extensions.Configuration/api/OpenCollar.Extensions.Configuration.ServiceCollectionExtensions.AddConfigurationObjectValidator.html) method to add
a class that implements the
[`IConfigurationObjectValidator`](https://open-collar.github.io/OpenCollar.Extensions.Configuration/api/OpenCollar.Extensions.Configuration.IConfigurationObjectValidator.html)
interface.  The
[`Validate`](https://open-collar.github.io/OpenCollar.Extensions.Configuration/api/OpenCollar.Extensions.Configuration.IConfigurationObjectValidator.Validate.html)
method is called every time a batch of changes completes.

# Related Projects

* [OpenCollar.Extensions](https://github.com/open-collar/OpenCollar.Extensions)
* [OpenCollar.Extensions.ApplicationInsights](https://github.com/open-collar/OpenCollar.Extensions.ApplicationInsights)
* [OpenCollar.Extensions.Collections](https://github.com/open-collar/OpenCollar.Extensions.Collections)
* [OpenCollar.Extensions.Configuraton](https://github.com/open-collar/OpenCollar.Extensions.Configuraton)
* [OpenCollar.Extensions.Environment](https://github.com/open-collar/OpenCollar.Extensions.Environment)
* [OpenCollar.Extensions.IO](https://github.com/open-collar/OpenCollar.Extensions.IO)
* [OpenCollar.Extensions.Logging](https://github.com/open-collar/OpenCollar.Extensions.Logging)
* [OpenCollar.Extensions.Security](https://github.com/open-collar/OpenCollar.Extensions.Security)
* [OpenCollar.Extensions.Threading](https://github.com/open-collar/OpenCollar.Extensions.Threading)
* [OpenCollar.Extensions.Validation](https://github.com/open-collar/OpenCollar.Extensions.Validation)