using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenCollar.Extensions.Configuration.Converters.Newtonsoft.Json
{
    public sealed class ConfigurationDictionaryConverter<TElement> : global::Newtonsoft.Json.JsonConverter<IConfigurationDictionary<TElement>>
    {
        public override IConfigurationDictionary<TElement> ReadJson(global::Newtonsoft.Json.JsonReader reader, Type objectType, [AllowNull] IConfigurationDictionary<TElement> existingValue, bool hasExistingValue, global::Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(global::Newtonsoft.Json.JsonWriter writer, [AllowNull] IConfigurationDictionary<TElement> value, global::Newtonsoft.Json.JsonSerializer serializer)
        {
            if(ReferenceEquals(value, null))
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            foreach(var item in value)
            {
                writer.WritePropertyName(item.Key);
                serializer.Serialize(writer, item.Value);
            }

            writer.WriteEndObject();
        }
    }
}