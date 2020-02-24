# Persistence

All configuration values are loaded on when the object is created.  Configuration reload events are
subscribed to, and any change to properties are reported using [Events](events.md) such as property
changed and collection changed.

To determine whether there any unsaved changes check the 
[IsDirty](api/OpenCollar.Extensions.Configuration.IConfigurationObject.html#OpenCollar_Extensions_Configuration_IConfigurationObject_IsDirty)
propery.

## Save

Changed made to properties and collections are not commited to the underlying configuration sources
until the [Save](api/OpenCollar.Extensions.Configuration.IConfigurationObject.html#OpenCollar_Extensions_Configuration_IConfigurationObject_Save)
method.

## Load

Any changes made since the configuration was loaded can reloaded using the [Load](api/OpenCollar.Extensions.Configuration.IConfigurationObject.html#OpenCollar_Extensions_Configuration_IConfigurationObject_Load)
method.

## Delete

At any time a node and it's children can be removed from the underlying configuration provider using
the [Delete](api/OpenCollar.Extensions.Configuration.IConfigurationObject.html#OpenCollar_Extensions_Configuration_IConfigurationObject_Delete)
method.