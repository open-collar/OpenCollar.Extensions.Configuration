# Collections, Dictionaries and Object

## Configuration Objects

Configuration objects are used to represent simple configuration in the form of an object hierarchy.

![UML Diagram of the ConfigurationObjectBase<> class](../images/uml-diagrams/ConfigurationObjectBase/ConfigurationObjectBase.svg)

## Collections

Collections are configuration objects that represents lists of the configuration
objects or naive values of the same type.  Values are identified by index.

![UML Diagram of the ConfigurationCollection<> class](../images/uml-diagrams/Collections/ConfigurationCollection/ConfigurationCollection.svg)

## Dictionaries

Dictionaries are configuration objects represents lists of the configuration
objects or naive values of the same type.  Values are referenced by name.

![UML Diagram of the ConfigurationDictionary<> class](../images/uml-diagrams/Collections/ConfigurationDictionary/ConfigurationDictionary.svg)

## "Naive" Types

Any type not derived from [IConfigurationObject](api/OpenCollar.Extensions.Configuration.IConfigurationObject.html)
is considered "naive" and will be serialized as a single value.