using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class PropertyValueTests
    {
        [Fact]
        public void TestConstructor()
        {
            using(var fixture = new ConfigurationFixture())
            {
                const string propertyName = "StringPropertyA";
                var def = new PropertyDef(typeof(IRootElement), typeof(IRootElement).GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
                var parent = new ConfigurationObjectMock(fixture.ConfigurationRoot, null);
                var x = new PropertyValue<string>(def, parent);

                Assert.NotNull(x);
                Assert.False(x.IsDirty);
                Assert.Equal(parent, x.Parent);
                Assert.Equal(propertyName, x.PropertyName);
                Assert.True(x.IsReadOnly);
            }
        }
    }
}