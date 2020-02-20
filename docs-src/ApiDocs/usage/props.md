# Properties


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

## Read-only

### Read-Only Collections

![UML Diagram of the ReadOnlyConfigurationCollection<> class](..\images\uml-diagrams\Collections\ReadOnlyConfigurationCollection\ReadOnlyConfigurationCollection.svg)


### Read-Only Dictionaries

![UML Diagram of the ReadOnlyConfigurationDictionary<> class](..\images\uml-diagrams\Collections\ReadOnlyConfigurationDictionary\ReadOnlyConfigurationDictionary.svg)

## Read-write

## Configuration Object Values
