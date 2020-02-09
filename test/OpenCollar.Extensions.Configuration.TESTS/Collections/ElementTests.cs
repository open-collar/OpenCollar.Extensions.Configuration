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

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS.Collections
{
    public sealed class ElementTests
    {
        [Fact]
        public void TestWithoutParent()
        {
            const string propertyName = "StringPropertyA";
            var def = new PropertyDef(typeof(IRootElement), typeof(IRootElement).GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));

            Assert.Throws<System.ArgumentNullException>(() =>
            {
                var x = new OpenCollar.Extensions.Configuration.Collections.Element<string, string>(def, null, "TEST") { Value = "TESTVALUE" };
            });
        }

        [Fact]
        public void TestWithParent()
        {
            using(var fixture = new ConfigurationFixture())
            {
                var parentObject = fixture.RootElement;

                var def = new PropertyDef(typeof(IRootElement), typeof(IRootElement).GetProperty(nameof(IRootElement.ChildDictionary), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));

                var x = new OpenCollar.Extensions.Configuration.Collections.Element<string, IChildElement>(def, (OpenCollar.Extensions.Configuration.Collections.ConfigurationDictionaryBase<string, IChildElement>)parentObject.ChildDictionary, "TEST");

                Assert.NotNull(x);
                Assert.Equal("TEST", x.Key);
                Assert.Equal("ChildDictionary:TEST", x.GetPath());
            }
        }
    }
}