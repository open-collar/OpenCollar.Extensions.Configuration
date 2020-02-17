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
using System.Reflection;

using OpenCollar.Extensions.Configuration.Collections;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS.Collections
{
    public sealed class ElementTests
    {
        [Fact]
        public void TestEquals()
        {
            using(var fixture = new ConfigurationFixture())
            {
                var def = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.StringPropertyA), BindingFlags.Public | BindingFlags.Instance));

                var elementA = new Element<string, IChildElement>(def, (ConfigurationDictionaryBase<string, IChildElement>)fixture.RootElement.ChildDictionary,
                "TEST1");
                var elementB = new Element<string, IChildElement>(def, (ConfigurationDictionaryBase<string, IChildElement>)fixture.RootElement.ChildDictionary,
                "TEST2");

                Assert.True(elementA.Equals(elementA));
                Assert.False(elementA.Equals((Element<string, IChildElement>)null));
                Assert.False(elementA.Equals(elementB));

                Assert.True(elementA.Equals((object)elementA));
                Assert.False(elementA.Equals((object)null));
                Assert.False(elementA.Equals((object)elementB));
                Assert.False(elementA.Equals("different type"));
            }
        }

        [Fact]
        public void TestWithoutParent()
        {
            const string propertyName = "StringPropertyA";
            var def = new PropertyDef(typeof(IRootElement).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance));

            Assert.Throws<ArgumentNullException>(() =>
            {
                var x = new Element<string, string>(def, null, "TEST")
                {
                Value = "TESTVALUE"
                };
            });
        }

        [Fact]
        public void TestWithParent()
        {
            using(var fixture = new ConfigurationFixture())
            {
                var parentObject = fixture.RootElement;

                var def = new PropertyDef(typeof(IRootElement).GetProperty(nameof(IRootElement.ChildDictionary), BindingFlags.Public | BindingFlags.Instance));

                var x = new Element<string, IChildElement>(def, (ConfigurationDictionaryBase<string, IChildElement>)parentObject.ChildDictionary, "TEST");

                Assert.NotNull(x);
                Assert.Equal("TEST", x.Key);
                Assert.Equal("ChildDictionary:TEST", x.CalculatePath());
            }
        }
    }
}