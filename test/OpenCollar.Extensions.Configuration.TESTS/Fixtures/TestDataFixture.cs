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

using System.Reflection;

using Moq;

namespace OpenCollar.Extensions.Configuration.TESTS.Collections
{
    public sealed partial class TestDataFixture : Disposable
    {
        public TestDataFixture()
        {
            BooleanPropertyDef = new PropertyDef(typeof(IRootElement).GetProperty("BooleanPropertyA", BindingFlags.Instance | BindingFlags.Public));
            ChildConfigurationDictionaryPropertyDef = new PropertyDef(
            typeof(IRootElement).GetProperty("ChildDictionary", BindingFlags.Instance | BindingFlags.Public));
            ChildConfigurationCollectionPropertyDef = new PropertyDef(
            typeof(IRootElement).GetProperty("ChildCollection", BindingFlags.Instance | BindingFlags.Public));
            ReadOnlyChildConfigurationDictionaryPropertyDef = new PropertyDef(
            typeof(IRootElement).GetProperty("ReadOnlyChildDictionary", BindingFlags.Instance | BindingFlags.Public));
            ReadOnlyChildConfigurationCollectionPropertyDef = new PropertyDef(
            typeof(IRootElement).GetProperty("ReadOnlyChildCollection", BindingFlags.Instance | BindingFlags.Public));

            Data = new[]
            {
            GetConfigurationObject("a", 0, false), GetConfigurationObject("b", 1, false), GetConfigurationObject("c", 2, false),
            GetConfigurationObject("d", 3, false), GetConfigurationObject("e", 4, true)
            };
        }

        public IPropertyDef ChildConfigurationCollectionPropertyDef
        {
            get;
        }

        public IPropertyDef ChildConfigurationDictionaryPropertyDef
        {
            get;
        }

        public IPropertyDef ReadOnlyChildConfigurationCollectionPropertyDef
        {
            get;
        }

        public IPropertyDef ReadOnlyChildConfigurationDictionaryPropertyDef
        {
            get;
        }

        private IPropertyDef BooleanPropertyDef
        {
            get;
        }

        private IChildElement[] Data
        {
            get;
        }

        internal TestDataContext GetContext()
        {
            return new TestDataContext(this);
        }

        private IChildElement GetConfigurationObject(string name, int value, bool isDirty)
        {
            var mockRoot = new Mock<IConfigurationParent>();
            var mock = mockRoot.As<IChildElement>();
            mockRoot.Setup(x => x.PropertyDef).Returns(new PropertyDef(typeof(IDummyInterface).GetProperty(name, BindingFlags.Instance | BindingFlags.Public)));
            mock.Setup(x => x.IsDirty).Returns(isDirty);
            mock.Setup(x => x.Name).Returns(name);
            mock.Setup(x => x.Value).Returns(value);
            return mock.Object;
        }
    }
}