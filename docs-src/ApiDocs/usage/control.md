# Fine Control

Fine control over properties is provided by two attributes:

 * [`ConfigurationAttribute`](https://open-collar.github.io/OpenCollar.Extensions.Configuration/api/OpenCollar.Extensions.Configuration.ConfigurationAttribute.html) -
   basic control over default value, loading and saving.
 * [`PathAttribute`](https://open-collar.github.io/OpenCollar.Extensions.Configuration/api/OpenCollar.Extensions.Configuration.PathAttribute.html) -
   the naming and path to the underlying configuration key.

## Configuration Attribute

The [Configuration](/api/OpenCollar.Extensions.Configuration.ConfigurationAttribute.html) attribute is used to control
default values and whether values are loaded or saved.

* `DefaultValue` - For properties that are not required to have a value in the configuration
                 provider a default value can be set here.  If no value is set a
                 [ConfigurationException](/api/OpenCollar.Extensions.Configuration.ConfigurationException.html)
                  will be thrown.
* `Persistence` - This property can be used to control whether values are loaded or saved.  The available
                    options are defined by the
                    [ConfigurationPersistenceActions](/api/OpenCollar.Extensions.Configuration.ConfigurationPersistenceActions.html)
                    enum.  The actions available are:

<table>
<tr>
<th>Name</th>
<th>Description</th>
</tr>
<tr>
<th>Ignore</th>
<td>Values will be neither loaded nor saved.</td>
</tr>
<tr>
<th>LoadAndSave</th>
<td>Values are loaded from the configuration service and changes are saved back to the configuration service.
This is the default behavior.</td>
</tr>
<tr>
<th>LoadOnly</th>
<td>Values are loaded from the configuration service, but changes are never saved back to the configuration service.</td>
</tr>
<tr>
<th>SaveOnly</th>
<td>Values are never loaded from the configuration service, but changes are saved back to the configuration service.
</td>
</tr>
</table>

## Path Attribute

The [Path](/api/OpenCollar.Extensions.Configuration.PathAttribute.html)
attribute allows the location of the configuration value to be determined.  There are two values 
defined in the [ConfigurationPersistenceActions](/api/OpenCollar.Extensions.Configuration.PathIs.html)
that can be assigned:


<table>
<tr>
<th>Name</th>
<th>Description</th>
</tr>
<tr>
<th>Root</th>
<td>The path is treated as a root and any previous context is ignored.  This allows a completely
arbitrary path to be used.</td>
</tr>
<tr>
<th>Suffix</th>
<td>The path is treated as a suffix to be applied (as part of colon delimited list) to the existing
path context.  This is the default behavior.</td>
</tr>
</table>