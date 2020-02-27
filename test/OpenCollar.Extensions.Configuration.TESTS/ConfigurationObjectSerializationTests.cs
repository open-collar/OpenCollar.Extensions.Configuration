using System.Text.Json;
using System.Linq;
using Xunit;
using OpenCollar.Extensions.Configuration.Converter;

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

            // For no obvious reason the string order of properties varies depending on whether the we run all or just
            // some tests.
            const string x = "{\"BooleanPropertyA\":true,\"BooleanPropertyB\":false,\"BytePropertyA\":1,\"BytePropertyB\":123,\"CharPropertyA\":\"a\",\"CharPropertyB\":\"B\",\"ChildCollection\":[{\"Name\":\"Item_0\",\"Value\":0},{\"Name\":\"Item_1\",\"Value\":1},{\"Name\":\"Item_2\",\"Value\":2}],\"ChildDictionary\":{\"Item1\":{\"Name\":\"Item_1\",\"Value\":1},\"Item2\":{\"Name\":\"Item_2\",\"Value\":2},\"Item3\":{\"Name\":\"Item_3\",\"Value\":3}},\"ChildElementProperty\":{\"Name\":\"NAME-1\",\"Value\":1},\"CustomProperty\":\"XX_XX\",\"CustomValueA\":\"CUSTOM-VALUE-A\",\"CustomValueB\":\"DEFAULT_VALUE\",\"CustomValueC\":\"DEFAULT_VALUE\",\"DateTimeOffsetPropertyA\":\"2020-01-10T18:00:30+03:00\",\"DateTimeOffsetPropertyB\":\"2019-10-01T14:30:15+03:00\",\"DateTimePropertyA\":\"2020-01-10T18:00:30\",\"DateTimePropertyB\":\"2019-10-01T14:30:15\",\"DecimalPropertyA\":555.666,\"DecimalPropertyB\":-666.777,\"DoublePropertyA\":555.666,\"DoublePropertyB\":-666.777,\"EnumPropertyA\":16,\"EnumPropertyB\":4,\"Int16PropertyA\":333,\"Int16PropertyB\":-444,\"Int32PropertyA\":333,\"Int32PropertyB\":-444,\"Int32PropertyC\":999,\"Int32PropertyD\":555,\"Int64PropertyA\":333,\"Int64PropertyB\":-444,\"NonFlagsEnumPropertyA\":1,\"NonFlagsEnumPropertyB\":3,\"ReadOnlyChildCollection\":[],\"ReadOnlyChildDictionary\":[],\"ReadOnlyCollection\":[],\"ReadOnlyDictionary\":[],\"SBytePropertyA\":99,\"SBytePropertyB\":-100,\"SinglePropertyA\":555.666,\"SinglePropertyB\":-666.777,\"SinglePropertyNoDefault\":-666.777,\"SinglePropertyWithDefault\":555.666,\"StringPropertyA\":\"111\",\"StringPropertyB\":\"222\",\"StringPropertyC\":\"222\",\"TimeSpanPropertyA\":{\"Ticks\":144100000000,\"Days\":0,\"Hours\":4,\"Milliseconds\":0,\"Minutes\":0,\"Seconds\":10,\"TotalDays\":0.1667824074074074,\"TotalHours\":4.002777777777778,\"TotalMilliseconds\":14410000,\"TotalMinutes\":240.16666666666666,\"TotalSeconds\":14410},\"TimeSpanPropertyB\":{\"Ticks\":18300000000,\"Days\":0,\"Hours\":0,\"Milliseconds\":0,\"Minutes\":30,\"Seconds\":30,\"TotalDays\":0.021180555555555557,\"TotalHours\":0.5083333333333333,\"TotalMilliseconds\":1830000,\"TotalMinutes\":30.5,\"TotalSeconds\":1830}}";
            const string y = "{\"BooleanPropertyA\":true,\"ChildDictionary\":{\"Item1\":{\"Name\":\"Item_1\",\"Value\":1},\"Item2\":{\"Name\":\"Item_2\",\"Value\":2},\"Item3\":{\"Name\":\"Item_3\",\"Value\":3}},\"BooleanPropertyB\":false,\"BytePropertyA\":1,\"BytePropertyB\":123,\"CharPropertyA\":\"a\",\"CharPropertyB\":\"B\",\"ChildCollection\":[{\"Name\":\"Item_0\",\"Value\":0},{\"Name\":\"Item_1\",\"Value\":1},{\"Name\":\"Item_2\",\"Value\":2}],\"ChildElementProperty\":{\"Name\":\"NAME-1\",\"Value\":1},\"CustomProperty\":\"XX_XX\",\"CustomValueA\":\"CUSTOM-VALUE-A\",\"CustomValueB\":\"DEFAULT_VALUE\",\"CustomValueC\":\"DEFAULT_VALUE\",\"DateTimeOffsetPropertyA\":\"2020-01-10T18:00:30+03:00\",\"DateTimeOffsetPropertyB\":\"2019-10-01T14:30:15+03:00\",\"DateTimePropertyA\":\"2020-01-10T18:00:30\",\"DateTimePropertyB\":\"2019-10-01T14:30:15\",\"DecimalPropertyA\":555.666,\"DecimalPropertyB\":-666.777,\"DoublePropertyA\":555.666,\"DoublePropertyB\":-666.777,\"EnumPropertyA\":16,\"EnumPropertyB\":4,\"Int16PropertyA\":333,\"Int16PropertyB\":-444,\"Int32PropertyA\":333,\"Int32PropertyB\":-444,\"Int32PropertyC\":999,\"Int32PropertyD\":555,\"Int64PropertyA\":333,\"Int64PropertyB\":-444,\"NonFlagsEnumPropertyA\":1,\"NonFlagsEnumPropertyB\":3,\"ReadOnlyChildCollection\":[],\"ReadOnlyChildDictionary\":[],\"ReadOnlyCollection\":[],\"ReadOnlyDictionary\":[],\"SBytePropertyA\":99,\"SBytePropertyB\":-100,\"SinglePropertyA\":555.666,\"SinglePropertyB\":-666.777,\"SinglePropertyNoDefault\":-666.777,\"SinglePropertyWithDefault\":555.666,\"StringPropertyA\":\"111\",\"StringPropertyB\":\"222\",\"StringPropertyC\":\"222\",\"TimeSpanPropertyA\":{\"Ticks\":144100000000,\"Days\":0,\"Hours\":4,\"Milliseconds\":0,\"Minutes\":0,\"Seconds\":10,\"TotalDays\":0.1667824074074074,\"TotalHours\":4.002777777777778,\"TotalMilliseconds\":14410000,\"TotalMinutes\":240.16666666666666,\"TotalSeconds\":14410},\"TimeSpanPropertyB\":{\"Ticks\":18300000000,\"Days\":0,\"Hours\":0,\"Milliseconds\":0,\"Minutes\":30,\"Seconds\":30,\"TotalDays\":0.021180555555555557,\"TotalHours\":0.5083333333333333,\"TotalMilliseconds\":1830000,\"TotalMinutes\":30.5,\"TotalSeconds\":1830}}";

            Assert.True(string.Equals(x, jsonString) || string.Equals(y, jsonString));
        }
    }
}