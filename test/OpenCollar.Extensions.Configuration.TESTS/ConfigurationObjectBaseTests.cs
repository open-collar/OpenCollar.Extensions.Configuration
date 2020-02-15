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
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Configuration;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public class ConfigurationObjectBaseTests : IClassFixture<ConfigurationFixture>
    {
        private readonly ConfigurationFixture _configurationFixture;

        public ConfigurationObjectBaseTests(ConfigurationFixture configurationFixture)
        {
            _configurationFixture = configurationFixture;
        }

        [Fact]
        public void TestCalculatePath()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(string.Empty, x.CalculatePath());

            var y = x.ChildCollection;

            var z = y[0];

            Assert.Equal("ChildCollection:0", z.CalculatePath());
        }

        [Fact]
        public void TestChangeEvents()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(999, x.Int32PropertyC);

            _configurationFixture.ConfigurationRoot[nameof(IRootElement.Int32PropertyC)] = "777";
            _configurationFixture.ConfigurationRoot.Reload();
            Assert.Equal(777, x.Int32PropertyC);

            Assert.Equal(555, x.Int32PropertyD);
            _configurationFixture.ConfigurationRoot[nameof(IRootElement.Int32PropertyD)] = "111";
            _configurationFixture.ConfigurationRoot.Reload();
            Assert.Equal(111, x.Int32PropertyD);

            string propertyName = null;

            x.PropertyChanged += (s, args) => { propertyName = args.PropertyName; };

            _configurationFixture.ConfigurationRoot[nameof(IRootElement.Int32PropertyC)] = "444";
            _configurationFixture.ConfigurationRoot.Reload();

            Assert.Equal(nameof(IRootElement.Int32PropertyC), propertyName);
            Assert.Equal(444, x.Int32PropertyC);
        }

        [Fact]
        public void TestConstructors()
        {
            var x = _configurationFixture.RootElement;

            Assert.NotNull(x);
            Assert.Null(x.PropertyDef());

            Assert.Throws<TargetInvocationException>(() =>
            {
                var y = Activator.CreateInstance(x.GetType(), (IConfigurationRoot)null, (IConfigurationParent)null);
            });

            try
            {
                var y = Activator.CreateInstance(x.GetType(), (IConfigurationRoot)null, (IConfigurationParent)null);
            }
            catch(TargetInvocationException ex)
            {
                Assert.IsType<ArgumentNullException>(ex.InnerException);
            }
            catch
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestDelete()
        {
            var fixture = new ConfigurationFixture();

            var valuesBefore = fixture.ConfigurationRoot.GetChildren().Where(s => !ReferenceEquals(s.Value, null)).Select(s => s.Path);

            var x = fixture.RootElement;

            x.Delete();

            var valuesAfter = fixture.ConfigurationRoot.GetChildren().ToDictionary(s => s.Path);

            foreach(var path in valuesBefore)
            {
                Assert.Null(valuesAfter[path].Value);
            }
        }

        [Fact]
        public void TestEquals()
        {
            var x = _configurationFixture.RootElement;
            var y = x.ChildCollection[0];
            var z = x.ChildCollection[1];

            Assert.True(x.Equals(x));
            Assert.False(x.Equals(y));
            Assert.False(y.Equals(x));
            Assert.False(x.Equals(z));
            Assert.False(x.Equals(null));
        }

        [Fact]
        public void TestProperties_Boolean()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(true, x.BooleanPropertyA);
            Assert.Equal(false, x.BooleanPropertyB);
            x.BooleanPropertyB = true;
            x.Save();
            x.Load();
            Assert.Equal(true, x.BooleanPropertyB);
        }

        [Fact]
        public void TestProperties_Char()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal('a', x.CharPropertyA);
            Assert.Equal('B', x.CharPropertyB);
            x.CharPropertyB = 'C';
            x.Save();
            x.Load();
            Assert.Equal('C', x.CharPropertyB);
        }

        [Fact]
        public void TestProperties_DateTime()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(new DateTime(2020, 01, 10, 18, 00, 30), x.DateTimePropertyA);
            Assert.Equal(new DateTime(2019, 10, 01, 14, 30, 15), x.DateTimePropertyB);
            var now = DateTime.UtcNow;
            x.DateTimePropertyB = now;
            x.Save();
            x.Load();
            Assert.Equal(now, x.DateTimePropertyB);
        }

        [Fact]
        public void TestProperties_DateTimeOffset()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(new DateTimeOffset(2020, 01, 10, 18, 00, 30, TimeSpan.FromHours(3)), x.DateTimeOffsetPropertyA);
            Assert.Equal(new DateTimeOffset(2019, 10, 01, 14, 30, 15, TimeSpan.FromHours(3)), x.DateTimeOffsetPropertyB);
            var now = DateTimeOffset.UtcNow;
            x.DateTimeOffsetPropertyB = now;
            x.Save();
            x.Load();
            Assert.Equal(now, x.DateTimeOffsetPropertyB);
        }

        [Fact]
        public void TestProperties_Decimal()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((decimal)555.666, x.DecimalPropertyA);
            Assert.Equal((decimal)-666.777, x.DecimalPropertyB);
            x.DecimalPropertyB = (decimal)-1111.2222;
            x.Save();
            x.Load();
            Assert.Equal((decimal)-1111.2222, x.DecimalPropertyB);
        }

        [Fact]
        public void TestProperties_Double()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(555.666, x.DoublePropertyA);
            Assert.Equal(-666.777, x.DoublePropertyB);
            x.DoublePropertyB = -1111.2222;
            x.Save();
            x.Load();
            Assert.Equal(-1111.2222, x.DoublePropertyB);
        }

        [Fact]
        public void TestProperties_Enum()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(BindingFlags.Public, x.EnumPropertyA);
            Assert.Equal(BindingFlags.Instance, x.EnumPropertyB);
            x.EnumPropertyB = BindingFlags.NonPublic;
            x.Save();
            x.Load();
            Assert.Equal(BindingFlags.NonPublic, x.EnumPropertyB);
        }

        [Fact]
        public void TestProperties_Int16()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((short)333, x.Int16PropertyA);
            Assert.Equal((short)-444, x.Int16PropertyB);
            x.Int16PropertyB = -1111;
            x.Save();
            x.Load();
            Assert.Equal((short)-1111, x.Int16PropertyB);
        }

        [Fact]
        public void TestProperties_Int32()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(333, x.Int32PropertyA);
            Assert.Equal(-444, x.Int32PropertyB);
            Assert.Equal(-444, x.Int32PropertyB);
            x.Int32PropertyB = -1111;
            x.Save();
            x.Load();
            Assert.Equal(-1111, x.Int32PropertyB);
        }

        [Fact]
        public void TestProperties_Int64()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(333, x.Int64PropertyA);
            Assert.Equal(-444, x.Int64PropertyB);
            Assert.Equal(-444, x.Int64PropertyB);
            x.Int64PropertyB = -1111;
            x.Save();
            x.Load();
            Assert.Equal(-1111, x.Int64PropertyB);
        }

        [Fact]
        public void TestProperties_Nulls()
        {
            var x = _configurationFixture.RootElement;

            _configurationFixture.ConfigurationRoot[nameof(IRootElement.SinglePropertyWithDefault)] = null;
            x.Load();
            Assert.Equal((float?)123.456, x.SinglePropertyWithDefault);

            _configurationFixture.ConfigurationRoot[nameof(IRootElement.SinglePropertyNoDefault)] = null;
            Assert.Throws<ConfigurationException>(() => x.Load());
        }

        [Fact]
        public void TestProperties_PathAttribute()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal("XX_XX", x.CustomProperty);
        }

        [Fact]
        public void TestProperties_SByte()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((sbyte)99, x.SBytePropertyA);
            Assert.Equal((sbyte)-100, x.SBytePropertyB);
            x.SBytePropertyB = -111;
            x.Save();
            x.Load();
            Assert.Equal((sbyte)-111, x.SBytePropertyB);
        }

        [Fact]
        public void TestProperties_Single()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((float)555.666, x.SinglePropertyA);
            Assert.Equal((float)-666.777, x.SinglePropertyB);
            x.SinglePropertyB = (float)-1111.2222;
            x.Save();
            x.Load();
            Assert.Equal((float)-1111.2222, x.SinglePropertyB);
        }

        [Fact]
        public void TestProperties_String()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal("111", x.StringPropertyA);
            Assert.Equal("222", x.StringPropertyB);
        }

        [Fact]
        public void TestProperties_TimeSpan()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(new TimeSpan(04, 00, 10), x.TimeSpanPropertyA);
            Assert.Equal(new TimeSpan(00, 30, 30), x.TimeSpanPropertyB);
            var seventeenSeconds = TimeSpan.FromSeconds(17);
            x.TimeSpanPropertyB = seventeenSeconds;
            x.Save();
            x.Load();
            Assert.Equal(seventeenSeconds, x.TimeSpanPropertyB); //It's a measure of time.
        }

        [Fact]
        public void TestPropertyEventsAndDirtyFlag()
        {
            var x = _configurationFixture.RootElement;

            Assert.False(x.IsDirty);
            string changedProperty = null;
            x.PropertyChanged += (source, args) => { changedProperty = args.PropertyName; };
            x.StringPropertyC = "SOMETHING ELSE";
            Assert.NotNull(changedProperty);
            Assert.Equal(nameof(IRootElement.StringPropertyC), changedProperty);
            Assert.True(x.IsDirty);
            Assert.Equal("SOMETHING ELSE", x.StringPropertyC);

            x.Save();

            Assert.False(x.IsDirty);
        }

        [Fact]
        public void TestSave()
        {
            var fixture = new ConfigurationFixture();

            var valuesBefore = fixture.ConfigurationRoot.GetChildren().Where(s => !ReferenceEquals(s.Value, null)).Select(s => s.Path);

            var x = fixture.RootElement;

            x.BooleanPropertyB = true;
            x.CharPropertyB = '!';
            var child = x.ChildCollection.AddNew();
            child.Name = "ABABAB";

            x.Save();

            Assert.Equal("True", fixture.ConfigurationRoot["BooleanPropertyB"]);
            Assert.Equal("!", fixture.ConfigurationRoot["CharPropertyB"]);
            Assert.Equal("ABABAB", fixture.ConfigurationRoot["ChildCollection:3:Name"]);
        }

        [Fact]
        public void TestToString()
        {
            var x = _configurationFixture.RootElement;

            var y = x.ChildCollection;

            var z1 = y[0];

            var z2 = y[1];

            Assert.IsAssignableFrom<IConfigurationObject>(x);
            Assert.IsAssignableFrom<IConfigurationObject>(z1);

            Assert.NotEqual(z1.ToString(), z2.ToString());

            Assert.Equal("OpenCollar.Extensions.Configuration.TESTS.IRootElement: \"\"", x.ToString());
            Assert.Equal("OpenCollar.Extensions.Configuration.TESTS.IChildElement: \"ChildCollection:0\"", z1.ToString());
            Assert.Equal("OpenCollar.Extensions.Configuration.TESTS.IChildElement: \"ChildCollection:1\"", z2.ToString());
        }
    }
}