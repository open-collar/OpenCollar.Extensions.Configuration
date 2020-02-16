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

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class PropertyDefTests
    {
        [Fact]
        public void TestCalculatePath()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.NonFlagsEnumPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(nameof(IRootElement.NonFlagsEnumPropertyA), propertyDef.CalculatePath(null));

            var parent = new ConfigurationParentMock();
            parent.Path = "TEST";

            Assert.Equal("TEST:" + nameof(IRootElement.NonFlagsEnumPropertyA), propertyDef.CalculatePath(parent));
        }

        [Fact]
        public void TestConvertStringToValue_Boolean()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.BooleanPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(true, (System.Boolean)propertyDef.ConvertStringToValue("PATH", "True"));
            Assert.Equal(false, (System.Boolean)propertyDef.ConvertStringToValue("PATH", "False"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN BOOLEAN"); });
        }

        [Fact]
        public void TestConvertStringToValue_Byte()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.BytePropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((System.Byte)123, propertyDef.ConvertStringToValue("PATH", "123"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "1234"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_Char()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.CharPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((System.Char)'x', propertyDef.ConvertStringToValue("PATH", "x"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "XX"); });
        }

        [Fact]
        public void TestConvertStringToValue_DateTime()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.DateTimePropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(new DateTime(2020, 12, 21, 12, 34, 56, DateTimeKind.Utc), propertyDef.ConvertStringToValue("PATH", "2020-12-21T12:34:56.0000000Z"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT A DATE"); });
        }

        [Fact]
        public void TestConvertStringToValue_DateTimeOffset()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.DateTimeOffsetPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(new DateTimeOffset(2020, 12, 21, 12, 34, 56, TimeSpan.FromHours(1)), propertyDef.ConvertStringToValue("PATH", "2020-12-21T12:34:56.0000000+01:00"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT A DATE"); });
        }

        [Fact]
        public void TestConvertStringToValue_Decimal()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.DecimalPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((System.Decimal)123456789.101112, propertyDef.ConvertStringToValue("PATH", "123456789.101112"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "123456785436547659012345678910.1234557567865867890"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_Double()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.DoublePropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((System.Double)123.456, (System.Double)propertyDef.ConvertStringToValue("PATH", "123.456"), 3);

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN NUMBER"); });
        }

        [Fact]
        public void TestConvertStringToValue_EnumFlags()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.EnumPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(BindingFlags.NonPublic, (BindingFlags)propertyDef.ConvertStringToValue("PATH", "NonPublic"));
            Assert.Equal(BindingFlags.CreateInstance, (BindingFlags)propertyDef.ConvertStringToValue("PATH", "CreateInstance"));
            Assert.Equal(BindingFlags.NonPublic, (BindingFlags)propertyDef.ConvertStringToValue("PATH", "32"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT A VALUE"); });
        }

        [Fact]
        public void TestConvertStringToValue_EnumNonFlags()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.NonFlagsEnumPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(NonFlagsEnum.First, (NonFlagsEnum)propertyDef.ConvertStringToValue("PATH", "First"));
            Assert.Equal(NonFlagsEnum.Second, (NonFlagsEnum)propertyDef.ConvertStringToValue("PATH", "2"));
            Assert.Equal(NonFlagsEnum.Third, (NonFlagsEnum)propertyDef.ConvertStringToValue("PATH", "3"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "5"); });
            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT A VALUE"); });
        }

        [Fact]
        public void TestConvertStringToValue_Int16()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.Int16PropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((System.Int16)12345, propertyDef.ConvertStringToValue("PATH", "12345"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "1234567890123"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_Int32()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.Int32PropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((System.Int32)123456789, propertyDef.ConvertStringToValue("PATH", "123456789"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "1234567890123"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_Int64()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.Int64PropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((System.Int64)123456789101112, propertyDef.ConvertStringToValue("PATH", "123456789101112"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "123456789012345678910"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_SByte()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.SBytePropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal((System.SByte)125, propertyDef.ConvertStringToValue("PATH", "125"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "1234567890123"); });

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN INT"); });
        }

        [Fact]
        public void TestConvertStringToValue_Single()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.SinglePropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(123.456, (System.Double)(System.Single)propertyDef.ConvertStringToValue("PATH", "123.456"), 3);

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT AN NUMBER"); });
        }

        [Fact]
        public void TestConvertStringToValue_String()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.StringPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal("test-string", propertyDef.ConvertStringToValue("PATH", "test-string"));
        }

        [Fact]
        public void TestConvertStringToValue_TimeSpan()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.TimeSpanPropertyA), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(System.TimeSpan.FromSeconds(1550), propertyDef.ConvertStringToValue("PATH", "00:25:50"));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", "NOT A DATE"); });
        }

        [Fact]
        public void TestDefaults()
        {
            var propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.SinglePropertyWithDefault), BindingFlags.Instance | BindingFlags.Public));

            Assert.Equal(123.456, (System.Double)(System.Single)propertyDef.ConvertStringToValue("PATH", null), 3);

            propertyDef = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.SinglePropertyNoDefault), BindingFlags.Instance | BindingFlags.Public));

            Assert.Throws<ConfigurationException>(() => { propertyDef.ConvertStringToValue("PATH", null); });
        }
    }
}