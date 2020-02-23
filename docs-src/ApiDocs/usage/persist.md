# Persistence


Later, when needed, the configuration reader is available as a service:

```cs
public MyConstructor(IMyConfig config)
{
    var version = config.Environment.Version;
}
```

## Save

## Load

## Delete