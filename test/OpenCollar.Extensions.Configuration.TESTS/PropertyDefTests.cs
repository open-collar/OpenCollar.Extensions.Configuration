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

using System;
using System.Reflection;

using OpenCollar.Extensions.Configuration.Collections;
using OpenCollar.Extensions.Configuration.TESTS.Collections;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class PropertyDefTests : IClassFixture<TestDataFixture>
    {
        private readonly TestDataFixture _propertyTestData;

        public PropertyDefTests(TestDataFixture propertyDefFixture)
        {
            _propertyTestData = propertyDefFixture;
        }

        [Fact]
        public void TestCalculatePath()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.NonFlagsEnumPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(nameof(IRootElement.NonFlagsEnumPropertyA), propertyDef.CalculatePath(null));

            var parent = new ConfigurationParentMock();
            parent.Path = "TEST";

            Assert.Equal("TEST:" + nameof(IRootElement.NonFlagsEnumPropertyA), propertyDef.CalculatePath(parent));
        }

        [Fact]
        public void TestConvertStringToValue_Boolean()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.BooleanPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(true, (bool)propertyDef.ConvertStringToValue("PATH", "True"));
            Assert.Equal(false, (bool)propertyDef.ConvertStringToValue("PATH", "False"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN BOOLEAN"); });
        }

        [Fact]
        public void TestConvertStringToValue_Byte()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.BytePropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((byte)123, propertyDef.ConvertStringToValue("PATH", "123"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "1234"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_Char()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.CharPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((char)'x', propertyDef.ConvertStringToValue("PATH", "x"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "XX"); });
        }

        [Fact]
        public void TestConvertStringToValue_DateTime()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.DateTimePropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(new DateTime(2020, 12, 21, 12, 34, 56, DateTimeKind.Utc), propertyDef.ConvertStringToValue("PATH", "2020-12-21T12:34:56.0000000Z"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT A DATE"); });
        }

        [Fact]
        public void TestConvertStringToValue_DateTimeOffset()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.DateTimeOffsetPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(new DateTimeOffset(2020, 12, 21, 12, 34, 56, TimeSpan.FromHours(1)),
            propertyDef.ConvertStringToValue("PATH", "2020-12-21T12:34:56.0000000+01:00"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT A DATE"); });
        }

        [Fact]
        public void TestConvertStringToValue_Decimal()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.DecimalPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((decimal)123456789.101112, propertyDef.ConvertStringToValue("PATH", "123456789.101112"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "123456785436547659012345678910.1234557567865867890"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_Double()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.DoublePropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((double)123.456, (double)propertyDef.ConvertStringToValue("PATH", "123.456"), 3);

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN NUMBER"); });
        }

        [Fact]
        public void TestConvertStringToValue_EnumFlags()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.EnumPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(BindingFlags.NonPublic, (BindingFlags)propertyDef.ConvertStringToValue("PATH", "NonPublic"));
            Assert.Equal(BindingFlags.CreateInstance, (BindingFlags)propertyDef.ConvertStringToValue("PATH", "CreateInstance"));
            Assert.Equal(BindingFlags.NonPublic, (BindingFlags)propertyDef.ConvertStringToValue("PATH", "32"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT A VALUE"); });
        }

        [Fact]
        public void TestConvertStringToValue_EnumNonFlags()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.NonFlagsEnumPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(NonFlagsEnum.First, (NonFlagsEnum)propertyDef.ConvertStringToValue("PATH", "First"));
            Assert.Equal(NonFlagsEnum.Second, (NonFlagsEnum)propertyDef.ConvertStringToValue("PATH", "2"));
            Assert.Equal(NonFlagsEnum.Third, (NonFlagsEnum)propertyDef.ConvertStringToValue("PATH", "3"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "5"); });
            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT A VALUE"); });
        }

        [Fact]
        public void TestConvertStringToValue_Int16()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.Int16PropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((short)12345, propertyDef.ConvertStringToValue("PATH", "12345"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "1234567890123"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_Int32()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.Int32PropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((int)123456789, propertyDef.ConvertStringToValue("PATH", "123456789"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "1234567890123"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_Int64()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.Int64PropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((long)123456789101112, propertyDef.ConvertStringToValue("PATH", "123456789101112"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "123456789012345678910"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_SByte()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.SBytePropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((sbyte)125, propertyDef.ConvertStringToValue("PATH", "125"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "1234567890123"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_Single()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.SinglePropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(123.456, (double)(float)propertyDef.ConvertStringToValue("PATH", "123.456"), 3);

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN NUMBER"); });
        }

        [Fact]
        public void TestConvertStringToValue_String()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.StringPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal("test-string", propertyDef.ConvertStringToValue("PATH", "test-string"));
        }

        [Fact]
        public void TestConvertStringToValue_TimeSpan()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.TimeSpanPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(TimeSpan.FromSeconds(1550), propertyDef.ConvertStringToValue("PATH", "00:25:50"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT A DATE"); });
        }

        [Fact]
        public void TestCopyValue()
        {
            var root = _propertyTestData.GetContext().Configuration.ConfigurationRoot;

            var propertyInfo = typeof(IRootElement).GetProperty(nameof(IRootElement.Int32PropertyB), BindingFlags.Instance | BindingFlags.Public);
            var propertyDef = new PropertyDef(propertyInfo);

            var copy = propertyDef.CopyValue(propertyDef.Implementation, 123, null, root);

            Assert.Equal(123, copy);
        }

        [Fact]
        public void TestCopyValue_Collection()
        {
            var root = _propertyTestData.GetContext().Configuration.ConfigurationRoot;

            var propertyInfo = typeof(IRootElement).GetProperty(nameof(IRootElement.ChildCollection), BindingFlags.Instance | BindingFlags.Public);
            var propertyDef = new PropertyDef(propertyInfo);

            var collection = new ConfigurationCollection<IChildElement>(null, propertyDef, root);

            collection.AddCopy(new ChildElementMock()
            {
                Name = "NAME-1",
                Value = 1
            });
            collection.AddCopy(new ChildElementMock()
            {
                Name = "NAME-2",
                Value = 2
            });
            collection.AddCopy(new ChildElementMock()
            {
                Name = "NAME-3",
                Value = 3
            });

            var copy = propertyDef.CopyValue(propertyDef.Implementation, collection, null, root);

            Assert.NotNull(copy);
            Assert.Equal(collection.Count, copy.Count);

            for(var n = 0; n < collection.Count; ++n)
            {
                Assert.NotNull(copy[n]);
                Assert.Equal(collection[n].Name, copy[n].Name);
                Assert.Equal(collection[n].Value, copy[n].Value);
            }
        }

        [Fact]
        public void TestCopyValue_Dictionary()
        {
            var root = _propertyTestData.GetContext().Configuration.ConfigurationRoot;

            var propertyInfo = typeof(IRootElement).GetProperty(nameof(IRootElement.ChildDictionary), BindingFlags.Instance | BindingFlags.Public);
            var propertyDef = new PropertyDef(propertyInfo);

            var dictionary = new ConfigurationDictionary<IChildElement>(null, propertyDef, root);

            dictionary.AddCopy("A", new ChildElementMock()
            {
                Name = "NAME-1",
                Value = 1
            });
            dictionary.AddCopy("B", new ChildElementMock()
            {
                Name = "NAME-2",
                Value = 2
            });
            dictionary.AddCopy("C", new ChildElementMock()
            {
                Name = "NAME-3",
                Value = 3
            });

            var copy = propertyDef.CopyValue(propertyDef.Implementation, dictionary, null, root);

            Assert.NotNull(copy);
            Assert.Equal(dictionary.Count, copy.Count);

            foreach(var k in dictionary.Keys)
            {
                Assert.NotNull(copy[k]);
                Assert.Equal(dictionary[k].Value.Name, copy[k].Value.Name);
                Assert.Equal(dictionary[k].Value.Value, copy[k].Value.Value);
            }
        }

        [Fact]
        public void TestCopyValue_Object()
        {
            var root = _propertyTestData.GetContext().Configuration.ConfigurationRoot;

            var propertyInfo = typeof(IRootElement).GetProperty(nameof(IRootElement.ChildElementProperty), BindingFlags.Instance | BindingFlags.Public);
            var propertyDef = new PropertyDef(propertyInfo);

            var child = new ChildElementMock()
            {
                Name = "NAME-1",
                Value = 1
            };

            var copy = propertyDef.CopyValue<IChildElement>(propertyDef.Implementation, child, null, root);

            Assert.NotNull(copy);
            Assert.Equal(child.Name, copy.Name);
            Assert.Equal(child.Value, copy.Value);

            copy = propertyDef.CopyValue<IChildElement>(propertyDef.Implementation, null, null, root);

            Assert.Null(copy);
        }

        [Fact]
        public void TestDefaults()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.SinglePropertyWithDefault), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(123.456, (double)(float)propertyDef.ConvertStringToValue("PATH", null), 3);

            propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.SinglePropertyNoDefault),
            BindingFlags.Instance | BindingFlags.Public));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", null); });
        }

        [Fact]
        public void TestIsReadOnly()
        {
            var propertyDef =
            new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.BooleanPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.True(propertyDef.IsReadOnly);

            propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.BooleanPropertyB), BindingFlags.Instance | BindingFlags.Public));

            Assert.False(propertyDef.IsReadOnly);
        }
    }
}