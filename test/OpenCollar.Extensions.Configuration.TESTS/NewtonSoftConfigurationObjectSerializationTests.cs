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

            var serializer = Newtonsoft.Json.JsonSerializer.Create(_settings);
            using(var textWriter = new StringWriter())
            {
                using(var jsonWriter = new JsonTextWriter(textWriter))
                {
                    serializer.Serialize(jsonWriter, childElement);

                    var jsonString = textWriter.ToString();

                    Assert.Equal("{\"Name\":\"Item_0\",\"Value\":0}", jsonString);
                }
            }
        }

        [Fact]
        public void TestComplexSerialization()
        {
            var fixture = new ConfigurationFixture();

            var rootElement = fixture.RootElement;

            var serializer = Newtonsoft.Json.JsonSerializer.Create(_settings);
            using(var textWriter = new StringWriter())
            {
                using(var jsonWriter = new JsonTextWriter(textWriter))
                {
                    serializer.Serialize(jsonWriter, rootElement);

                    var jsonString = textWriter.ToString();

                    const string x = "{\"BooleanPropertyA\":true,\"BooleanPropertyB\":false,\"BytePropertyA\":1,\"BytePropertyB\":123,\"CharPropertyA\":\"a\",\"CharPropertyB\":\"B\",\"ChildCollection\":[{\"Name\":\"Item_0\",\"Value\":0},{\"Name\":\"Item_1\",\"Value\":1},{\"Name\":\"Item_2\",\"Value\":2}],\"ChildDictionary\":{\"Item1\":{\"Name\":\"Item_1\",\"Value\":1},\"Item2\":{\"Name\":\"Item_2\",\"Value\":2},\"Item3\":{\"Name\":\"Item_3\",\"Value\":3}},\"ChildElementProperty\":{\"Name\":\"NAME-1\",\"Value\":1},\"CustomRoot\":\"XX_XX\",\"CustomValueA\":\"CUSTOM-VALUE-A\",\"CustomValueB\":\"DEFAULT_VALUE\",\"CustomValueC\":\"DEFAULT_VALUE\",\"DateTimeOffsetPropertyA\":\"2020-01-10T18:00:30+03:00\",\"DateTimeOffsetPropertyB\":\"2019-10-01T14:30:15+03:00\",\"DateTimePropertyA\":\"2020-01-10T18:00:30\",\"DateTimePropertyB\":\"2019-10-01T14:30:15\",\"DecimalPropertyA\":555.666,\"DecimalPropertyB\":-666.777,\"DoublePropertyA\":555.666,\"DoublePropertyB\":-666.777,\"EnumPropertyA\":16,\"EnumPropertyB\":4,\"Int16PropertyA\":333,\"Int16PropertyB\":-444,\"Int32PropertyA\":333,\"Int32PropertyB\":-444,\"Int32PropertyC\":999,\"Int32PropertyD\":555,\"Int64PropertyA\":333,\"Int64PropertyB\":-444,\"NonFlagsEnumPropertyA\":1,\"NonFlagsEnumPropertyB\":3,\"ReadOnlyChildCollection\":[],\"ReadOnlyChildDictionary\":{},\"ReadOnlyCollection\":[],\"ReadOnlyDictionary\":[],\"SBytePropertyA\":99,\"SBytePropertyB\":-100,\"SinglePropertyA\":555.666,\"SinglePropertyB\":-666.777,\"SinglePropertyNoDefault\":-666.777,\"SinglePropertyWithDefault\":555.666,\"StringPropertyA\":\"111\",\"StringPropertyB\":\"222\",\"StringPropertyC\":\"222\",\"TimeSpanPropertyA\":\"0.04:00:10:\",\"TimeSpanPropertyB\":\"0.00:30:30:\"}";

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