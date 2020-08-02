/*
 * This file is part of OpenCollar.Extensions.Configuration.
 *
 * OpenCollar.Extensions.Configuration is free software: you can redistribute it
 * and/or modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * OpenCollar.Extensions.Configuration is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public
 * License for more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * OpenCollar.Extensions.Configuration.  If not, see <https://www.gnu.org/licenses/>.
 *
 * Copyright © 2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

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