# Events

Any object returned implements the
[INotifyPropertyChanged](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=netstandard-2.1)
interface, allowing for changes to properties (whether from the underlying
configuration or from property changes made by code) to be detected.

Similary collections and dictionaries implement the
[INotifyCollectionChanged](https://docs.microsoft.com/en-us/dotnet/api/system.collections.specialized.inotifycollectionchanged?view=netstandard-2.1)
interface, similarly allowing changes to be detected, regardless of origin.

## Property Change Events

## Collection Change Events