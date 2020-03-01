# JSON Serialization

The following example shows how to use the System.Text.Json serializer to convert
any configuration object into JSON.

```cs
// Create serializer (with dictionary converter).
var serializerOptions = new JsonSerializerOptions
{
    Converters =
                {
                    new ConfigurationDictionaryConverterFactory()
                }
};

var jsonString = JsonSerializer.Serialize(myConfigObject, serializerOptions);
```

Until [#31007](https://github.com/dotnet/runtime/issues/31007) is fixed in CoreFX 
we must manually add the converter when constructing the serializer.  The correct
attributes have been applied to the properties and classes involved, but are not
currently used as they should be.