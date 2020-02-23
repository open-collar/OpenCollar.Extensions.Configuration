# Events

Any object returned implements the
[INotifyPropertyChanged](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=netstandard-2.1)
interface, allowing for changes to properties (whether from the underlying
configuration or from property changes made by code) to be detected.

In addition, collections and dictionaries implement the
[INotifyCollectionChanged](https://docs.microsoft.com/en-us/dotnet/api/system.collections.specialized.inotifycollectionchanged?view=netstandard-2.1)
interface, similarly allowing changes to be detected, regardless of origin.

## Property Change Events

Property change are detected using the 
[PropertyChanged](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged?view=netstandard-2.1)
event.  A typical example looks like this:

```cs
public MyConstructor(IMyConfig config)
{
    config.PropertyChanged += OnConfigPropertyChanged;
}

private OnConfigPropertyChanged(object sender, PropertyChangedEventArgs arg)
{
    if(args.Name == "Environment")
    {
        // Do something here...
    }
}
```

## Collection Change Events


Collection and dictionary changes are detected using the 
[CollectionChanged](https://docs.microsoft.com/en-us/dotnet/api/system.collections.specialized.inotifycollectionchanged.collectionchanged?view=netstandard-2.1)
event.  A typical example looks like this:

```cs
public MyConstructor(IMyConfig config)
{
    config.ConnectionStrings.CollectionChanged += OnConnectionStringsChanged;
}

private OnConnectionStringsChanged(object sender, NotifyCollectionChangedEventArgs arg)
{
    if(args.Action == NotifyCollectionChangedAction.Add)
    {
        // Do something here...
    }
}
```