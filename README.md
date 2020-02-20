![Open Collar](./media/opencollar-logo-320x25x32.png) 
# OpenCollar.Extensions.Configuration

<table style="border-style: none; width: 100%;">
    <tr style="border-style: none;">
        <td style="width: 20%; border-style: none;">Latest Build:</td>
        <td style="width: 20%; border-style: none;"><a href="https://github.com/open-collar/OpenCollar.Extensions.Configuration/actions"><img src="https://img.shields.io/github/workflow/status/open-collar/OpenCollar.Extensions.Configuration/Build and Deploy"/></a></td>
        <td style="width: 20%; border-style: none;"><a href="https://coveralls.io/github/open-collar/OpenCollar.Extensions.Configuration?branch=master"><img src="https://coveralls.io/repos/github/open-collar/OpenCollar.Extensions.Configuration/badge.svg?branch=master"/></a></td>
        <td style="width: 20%; border-style: none;"><a href="https://www.nuget.org/packages/OpenCollar.Extensions.Configuration/"><img src="https://img.shields.io/nuget/vpre/OpenCollar.Extensions.Configuration?color=green"/></a></td>
        <td style="width: 20%; border-style: none;"><a href="https://open-collar.github.io/OpenCollar.Extensions.Configuration/articles/intro.html"><img src="https://img.shields.io/nuget/dt/OpenCollar.Extensions.Configuration?color=green"/></a></td>
    </tr>
</table>

 * [Source Code](https://github.com/open-collar/OpenCollar.Extensions.Configuration)
 * [Issue Tracking](https://github.com/open-collar/OpenCollar.Extensions.Configuration/issues)
 * [GitHub Pages](https://open-collar.github.io/OpenCollar.Extensions.Configuration/)

The purpose of the libarary is to provide a strongly typed, validated
wrapper around the values exposed as key/value pairs by configuration
providers in the 
[Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1)
library.

The starting point is to define an interface through which to read your 
configuration.  The interface must derive from
[IConfigurationObject](/api/OpenCollar.Extensions.Configuration.IConfigurationObject.md).
The interfaces must be public.  For example:

```
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

```
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

```
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

See "[Events](https://open-collar.github.io/OpenCollar.Extensions.Configuration/usage/events.md)" for a more detailed review of the events and when they are raised.

## Fine Contol

Fine control over properties is provided by three attributes:

 * [`ConfigurationAttribute`](https://open-collar.github.io/OpenCollar.Extensions.Configuration/api/OpenCollar.Extensions.Configuration.ConfigurationAttribute.html) -
   basic control over default value, loading and saving.
 * [`PathAttribute`](https://open-collar.github.io/OpenCollar.Extensions.Configuration/api/OpenCollar.Extensions.Configuration.PathAttribute.html) -
   the naming and path to the underlying configuration key.

The "[Fine Control](https://open-collar.github.io/OpenCollar.Extensions.Configuration/usage/control.md)" article looks in more depth at how attributes can be used to 
control the way in which configuration data is used.

 * [API Documentation](https://open-collar.github.io/OpenCollar.Extensions.Configuration/)
 * View package in [nuget.org](https://nuget.org) at: https://www.nuget.org/packages/OpenCollar.Extensions.Configuration/

## Set-up Developer Environment

There are no hard dependencies on tooling, all that is required is the 
.NET Core 3.1 SDK which can be installed:

 * From the [SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) page;
 * Or as part of Visual Studio 2019 (Versions >= 16.4.0) (the
   [Community Edition](https://visualstudio.microsoft.com/vs/community/) is
   free).

### Non-Essential Tools

 * Visual Studio Extensions:
     * [CodeMaid](http://www.codemaid.net/) - very handy for standardising
       formatting;
     * [EditorGuideLines](https://marketplace.visualstudio.com/items?itemName=PaulHarrington.EditorGuidelines) -
       simple vertical lines in the VS text editor to help with alignment and line length;
     * [FxCop](https://docs.microsoft.com/en-us/visualstudio/code-quality/install-fxcop-analyzers?view=vs-2019#to-install-fxcop-analyzers-as-a-vsix) -
       static code analysis;
     * [GhostDoc](https://submain.com/products/ghostdoc.aspx) - XML comment
       generation and editing;
     * [ILSpy](https://marketplace.visualstudio.com/items?itemName=SharpDevelopTeam.ILSpy) -
       code decompilation;
     * [Markdown Editor](https://github.com/madskristensen/MarkdownEditor) -
       VS editor for Markdown files;
     * [SpecFlow](https://specflow.org/) - acceptance testing;
 * Visual Studio Code Extensions:
   * [Jebbs PlantUML](https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml) - UML diagram generation.

## Reference

 * [Options Pattern in .NET Core](https://codeburst.io/options-pattern-in-net-core-a50285aeb18d);
 * [Tutorial: Express your design intent more clearly with nullable and non-nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/nullable-reference-types);
 * [Mastering Markdown](https://guides.github.com/features/mastering-markdown/)
 * [Coveralls GitHub Action](https://github.com/marketplace/actions/coveralls-github-action)
 * [Badges](https://shields.io/category/build)
