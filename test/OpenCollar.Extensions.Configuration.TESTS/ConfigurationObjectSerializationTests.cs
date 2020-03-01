using System.Text.Json;
using System.Linq;
using Xunit;
using OpenCollar.Extensions.Configuration.Converters;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public class ConfigurationObjectSerializationTests : IClassFixture<ConfigurationFixture>
    {
        private readonly ConfigurationFixture _configurationFixture;

        public ConfigurationObjectSerializationTests(ConfigurationFixture configurationFixture)
        {
            _configurationFixture = configurationFixture;
        }

        [Fact]
        public void TestBasicSerialization()
        {
            var childElement = _configurationFixture.RootElement.ChildCollection.First();

            var jsonString = JsonSerializer.Serialize(childElement);

            Assert.Equal("{\"Name\":\"Item_0\",\"Value\":0}", jsonString);
        }

        [Fact]
        public void TestComplexSerialization()
        {
            // BUG: Until this is fixed in corefx we must manually add the converter: https://github.com/dotnet/runtime/issues/31007
            var serializerOptions = new JsonSerializerOptions
            {
                Converters =
                           {
                              new ConfigurationDictionaryConverterFactory()
                           }
            };

            var fixture = new ConfigurationFixture();

            var rootElement = fixture.RootElement;

            var jsonString = JsonSerializer.Serialize(rootElement, serializerOptions);

            const string x = "{\"BooleanPropertyA\":true,\"BooleanPropertyB\":false,\"BytePropertyA\":1,\"BytePropertyB\":123,\"CharPropertyA\":\"a\",\"CharPropertyB\":\"B\",\"ChildCollection\":[{\"Name\":\"Item_0\",\"Value\":0},{\"Name\":\"Item_1\",\"Value\":1},{\"Name\":\"Item_2\",\"Value\":2}],\"ChildDictionary\":{\"Item1\":{\"Name\":\"Item_1\",\"Value\":1},\"Item2\":{\"Name\":\"Item_2\",\"Value\":2},\"Item3\":{\"Name\":\"Item_3\",\"Value\":3}},\"ChildElementProperty\":{\"Name\":\"NAME-1\",\"Value\":1},\"CustomProperty\":\"XX_XX\",\"CustomValueA\":\"CUSTOM-VALUE-A\",\"CustomValueB\":\"DEFAULT_VALUE\",\"CustomValueC\":\"DEFAULT_VALUE\",\"DateTimeOffsetPropertyA\":\"2020-01-10T18:00:30+03:00\",\"DateTimeOffsetPropertyB\":\"2019-10-01T14:30:15+03:00\",\"DateTimePropertyA\":\"2020-01-10T18:00:30\",\"DateTimePropertyB\":\"2019-10-01T14:30:15\",\"DecimalPropertyA\":555.666,\"DecimalPropertyB\":-666.777,\"DoublePropertyA\":555.666,\"DoublePropertyB\":-666.777,\"EnumPropertyA\":16,\"EnumPropertyB\":4,\"Int16PropertyA\":333,\"Int16PropertyB\":-444,\"Int32PropertyA\":333,\"Int32PropertyB\":-444,\"Int32PropertyC\":999,\"Int32PropertyD\":555,\"Int64PropertyA\":333,\"Int64PropertyB\":-444,\"NonFlagsEnumPropertyA\":1,\"NonFlagsEnumPropertyB\":3,\"ReadOnlyChildCollection\":[],\"ReadOnlyChildDictionary\":[],\"ReadOnlyCollection\":[],\"ReadOnlyDictionary\":[],\"SBytePropertyA\":99,\"SBytePropertyB\":-100,\"SinglePropertyA\":555.666,\"SinglePropertyB\":-666.777,\"SinglePropertyNoDefault\":-666.777,\"SinglePropertyWithDefault\":555.666,\"StringPropertyA\":\"111\",\"StringPropertyB\":\"222\",\"StringPropertyC\":\"222\",\"TimeSpanPropertyA\":{\"Ticks\":144100000000,\"Days\":0,\"Hours\":4,\"Milliseconds\":0,\"Minutes\":0,\"Seconds\":10,\"TotalDays\":0.1667824074074074,\"TotalHours\":4.002777777777778,\"TotalMilliseconds\":14410000,\"TotalMinutes\":240.16666666666666,\"TotalSeconds\":14410},\"TimeSpanPropertyB\":{\"Ticks\":18300000000,\"Days\":0,\"Hours\":0,\"Milliseconds\":0,\"Minutes\":30,\"Seconds\":30,\"TotalDays\":0.021180555555555557,\"TotalHours\":0.5083333333333333,\"TotalMilliseconds\":1830000,\"TotalMinutes\":30.5,\"TotalSeconds\":1830}}";

            var objX = System.Text.Json.JsonDocument.Parse(x);
            var objY = System.Text.Json.JsonDocument.Parse(jsonString);

            var bagX = JsonDocumentToPropertyBag(objX);
            var bagY = JsonDocumentToPropertyBag(objY);

            // We can't just compare strings because the ordering of properties is not guaranteed, so instead we must
            // deserialize the strings and use a deep equals that does not pay attention to property ordering.
            Assert.True(Equals(bagX, bagY));
        }

        private static bool Equals(System.Collections.Generic.Dictionary<string, object> a, System.Collections.Generic.Dictionary<string, object> b)
        {
            if(ReferenceEquals(a, b))
            {
                return true;
            }
            if(ReferenceEquals(a, null))
            {
                return false;
            }
            if(ReferenceEquals(b, null))
            {
                return false;
            }
            if(a.Count != b.Count)
            {
                return false;
            }
            foreach(var pair in a)
            {
                if(!b.TryGetValue(pair.Key, out var bValue))
                {
                    return false;
                }
                if(!Equals(pair.Value, bValue))
                {
                    return false;
                }
            }

            return true;
        }

        private static System.Collections.Generic.Dictionary<string, object> JsonDocumentToPropertyBag(System.Text.Json.JsonDocument document)
        {
            var bag = new System.Collections.Generic.Dictionary<string, object>();

            JsonElementToPropertyBag(document.RootElement, string.Empty, bag);

            return bag;
        }

        private static void JsonElementToPropertyBag(System.Text.Json.JsonElement element, string path, System.Collections.Generic.Dictionary<string, object> bag)
        {
            switch(element.ValueKind)
            {
                case JsonValueKind.Array:
                    int index = 0;
                    foreach(var arrayElement in element.EnumerateArray())
                    {
                        JsonElementToPropertyBag(arrayElement, string.Concat(path, ":", index++.ToString(System.Globalization.CultureInfo.InvariantCulture)), bag);
                    }
                    break;

                case JsonValueKind.False:
                    bag.Add(path, false);
                    break;

                case JsonValueKind.Null:
                    bag.Add(path, null);
                    break;

                case JsonValueKind.Number:
                    bag.Add(path, element.GetDouble());
                    break;

                case JsonValueKind.Object:
                    foreach(var property in element.EnumerateObject())
                    {
                        var name = string.IsNullOrEmpty(path) ? property.Name : string.Concat(path, ":", property.Name);

                        JsonElementToPropertyBag(property.Value, name, bag);
                    }
                    break;

                case JsonValueKind.String:
                    bag.Add(path, element.GetString());
                    break;

                case JsonValueKind.True:
                    bag.Add(path, true);
                    break;
            }
        }
    }
}