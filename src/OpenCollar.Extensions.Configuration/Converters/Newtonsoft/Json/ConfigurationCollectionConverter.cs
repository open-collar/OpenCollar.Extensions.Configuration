using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration.Converters.Newtonsoft.Json
{
    public sealed class ConfigurationCollectionConverter<TElement> : global::Newtonsoft.Json.JsonConverter<IConfigurationCollection<TElement>>
    {
        public override IConfigurationCollection<TElement> ReadJson(global::Newtonsoft.Json.JsonReader reader, Type objectType, [AllowNull] IConfigurationCollection<TElement> existingValue, bool hasExistingValue, global::Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(global::Newtonsoft.Json.JsonWriter writer, [AllowNull] IConfigurationCollection<TElement> value, global::Newtonsoft.Json.JsonSerializer serializer)
        {
            if(ReferenceEquals(value, null))
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartArray();

            foreach(var item in value)
            {
                if(ReferenceEquals(value, null))
                {
                    writer.WriteNull();
                }
                else
                {
                    serializer.Serialize(writer, item);
                }
            }

            writer.WriteEndArray();
        }
    }
}