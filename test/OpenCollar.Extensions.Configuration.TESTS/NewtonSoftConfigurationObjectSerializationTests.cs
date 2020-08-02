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
 * Copyright © 2019-2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public class NewtonsoftConfigurationObjectSerializationTests : IClassFixture<ConfigurationFixture>
    {
        private readonly ConfigurationFixture _configurationFixture;

        private JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.None
        };

        public NewtonsoftConfigurationObjectSerializationTests(ConfigurationFixture configurationFixture)
        {
            _configurationFixture = configurationFixture;
        }

        [Fact]
        public void TestBasicSerialization()
        {
            var childElement = _configurationFixture.RootElement.ChildCollection.First();

            var serializer = JsonSerializer.Create(_settings);
            using(var textWriter = new StringWriter())
            {
                using(var jsonWriter = new JsonTextWriter(textWriter))
                {
                    serializer.Serialize(jsonWriter, childElement);

                    var jsonString = textWriter.ToString();

                    Assert.Equal("{\"name\":\"Item_0\",\"value\":0}", jsonString);
                }
            }
        }

        [Fact]
        public void TestComplexSerialization()
        {
            var fixture = new ConfigurationFixture();

            var rootElement = fixture.RootElement;

            var serializer = JsonSerializer.Create(_settings);
            using(var textWriter = new StringWriter())
            {
                using(var jsonWriter = new JsonTextWriter(textWriter))
                {
                    serializer.Serialize(jsonWriter, rootElement);

                    var jsonString = textWriter.ToString();

                    const string x = "{\"booleanPropertyA\":true,\"booleanPropertyB\":false,\"bytePropertyA\":1,\"bytePropertyB\":123,\"charPropertyA\":\"a\",\"charPropertyB\":\"B\",\"childCollection\":[{\"name\":\"Item_0\",\"value\":0},{\"name\":\"Item_1\",\"value\":1},{\"name\":\"Item_2\",\"value\":2}],\"childDictionary\":{\"Item1\":{\"name\":\"Item_1\",\"value\":1},\"Item2\":{\"name\":\"Item_2\",\"value\":2},\"Item3\":{\"name\":\"Item_3\",\"value\":3}},\"childElementProperty\":{\"name\":\"NAME-1\",\"value\":1},\"customProperty\":\"XX_XX\",\"customValueA\":\"CUSTOM-VALUE-A\",\"customValueB\":\"DEFAULT_VALUE\",\"customValueC\":\"DEFAULT_VALUE\",\"dateTimeOffsetPropertyA\":\"2020-01-10T18:00:30+03:00\",\"dateTimeOffsetPropertyB\":\"2019-10-01T14:30:15+03:00\",\"dateTimePropertyA\":\"2020-01-10T18:00:30\",\"dateTimePropertyB\":\"2019-10-01T14:30:15\",\"decimalPropertyA\":555.666,\"decimalPropertyB\":-666.777,\"doublePropertyA\":555.666,\"doublePropertyB\":-666.777,\"enumPropertyA\":16,\"enumPropertyB\":4,\"int16PropertyA\":333,\"int16PropertyB\":-444,\"int32PropertyA\":333,\"int32PropertyB\":-444,\"int32PropertyC\":999,\"int32PropertyD\":555,\"int64PropertyA\":333,\"int64PropertyB\":-444,\"nonFlagsEnumPropertyA\":1,\"nonFlagsEnumPropertyB\":3,\"readOnlyChildCollection\":[],\"readOnlyChildDictionary\":{},\"readOnlyCollection\":[],\"readOnlyDictionary\":[],\"sBytePropertyA\":99,\"sBytePropertyB\":-100,\"singlePropertyA\":555.666,\"singlePropertyB\":-666.777,\"singlePropertyNoDefault\":-666.777,\"singlePropertyWithDefault\":555.666,\"stringPropertyA\":\"111\",\"stringPropertyB\":\"222\",\"stringPropertyC\":\"222\",\"timeSpanPropertyA\":\"0.04:00:10:\",\"timeSpanPropertyB\":\"0.00:30:30:\"}";

                    var objX = JToken.Parse(x);
                    var objY = JToken.Parse(jsonString);

                    var bagX = JsonDocumentToPropertyBag(objX);
                    var bagY = JsonDocumentToPropertyBag(objY);

                    // We can't just compare strings because the ordering of properties is not guaranteed, so instead we
                    // must deserialize the strings and use a deep equals that does not pay attention to property ordering.
                    Assert.True(EqualityHelper.Equals(bagX, bagY));
                }
            }
        }

        private static System.Collections.Generic.Dictionary<string, object> JsonDocumentToPropertyBag(JToken document)
        {
            var bag = new System.Collections.Generic.Dictionary<string, object>();

            JsonElementToPropertyBag(document, string.Empty, bag);

            return bag;
        }

        private static void JsonElementToPropertyBag(JToken element, string path, System.Collections.Generic.Dictionary<string, object> bag)
        {
            JValue value;
            switch(element.Type)
            {
                case JTokenType.Array:
                    int index = 0;
                    var array = element as JArray;
                    foreach(var arrayElement in array)
                    {
                        JsonElementToPropertyBag(arrayElement, string.Concat(path, ":", index++.ToString(System.Globalization.CultureInfo.InvariantCulture)), bag);
                    }
                    break;

                case JTokenType.Boolean:
                    value = element as JValue;
                    bag.Add(path, value.Value<bool>());
                    break;

                case JTokenType.Null:
                    bag.Add(path, null);
                    break;

                case JTokenType.Integer:
                    value = element as JValue;
                    bag.Add(path, value.Value<long>());
                    break;

                case JTokenType.Float:
                    value = element as JValue;
                    bag.Add(path, value.Value<double>());
                    break;

                case JTokenType.Object:
                    var obj = element as JObject;
                    foreach(var property in obj.Properties())
                    {
                        var name = string.IsNullOrEmpty(path) ? property.Name : string.Concat(path, ":", property.Name);

                        JsonElementToPropertyBag(property.Value, name, bag);
                    }
                    break;

                case JTokenType.String:
                    value = element as JValue;
                    bag.Add(path, value.Value<string>());
                    break;
            }
        }
    }
}