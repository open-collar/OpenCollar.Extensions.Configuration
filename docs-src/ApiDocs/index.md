Support for automatic validation, update and strongly-typed access to configuration.

# Where to Start

The starting point is to define an interface through which to read your configuration and add a reader in `Startup.cs`:

```
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages();
    services.AddConfigurationReader<IMyConfig>();
}
```

Later, when needed the configuration reader will be available as a service:

```
public MyConstructor(IMyConfig config)
{
    // Your config is available through the interface you defined in IMyConfig, strongly typed and validated.
}
```

# Project

## Project Resources

 * [Source Code](https://github.com/open-collar/OpenCollar.Extensions.Configuration)
 * [Issue Tracking](https://github.com/open-collar/OpenCollar.Extensions.Configuration/issues)
 * [GitHub Pages](https://open-collar.github.io/OpenCollar.Extensions.Configuration/)

## Build Status
* ![Master](https://github.com/open-collar/OpenCollar.Extensions.Configuration/workflows/.NET%20Core/badge.svg)