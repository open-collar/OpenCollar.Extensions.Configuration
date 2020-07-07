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