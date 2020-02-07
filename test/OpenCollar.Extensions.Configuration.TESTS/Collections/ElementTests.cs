using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS.Collections
{
    public sealed class ElementTests
    {
        [Fact]
        public void TestConstructor()
        {
            using(var fixture = new ConfigurationFixture())
            {
                const string propertyName = "StringPropertyA";
                var def = new PropertyDef(typeof(IRootElement), typeof(IRootElement).GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
                var parent = new ConfigurationDictionary<string>(null, def, fixture.ConfigurationRoot, null);
                var x = new OpenCollar.Extensions.Configuration.Collections.Element<string, string>(def, parent, "TEST", "TESTVALUE");

                Assert.NotNull(x);
                Assert.Equal("TEST", x.Key);
                Assert.Equal("StringPropertyA:TEST", x.GetPath());
            }
        }
    }
}