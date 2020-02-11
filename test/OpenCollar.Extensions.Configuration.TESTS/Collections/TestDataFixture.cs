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

using System.Collections.Generic;
using System.Reflection;

using Moq;

namespace OpenCollar.Extensions.Configuration.TESTS.Collections
{
    public interface IDummyInterface
    {
        IRootElement a { get; set; }

        IRootElement b { get; set; }

        IRootElement c { get; set; }

        IRootElement d { get; set; }

        IRootElement e { get; set; }
    }

    public sealed class TestDataFixture : Disposable
    {
        public TestDataFixture()
        {
            BooleanPropertyDef = new PropertyDef(typeof(IRootElement),
                typeof(IRootElement).GetProperty("BooleanPropertyA", BindingFlags.Instance | BindingFlags.Public));
            ChildConfigurationDictionaryPropertyDef = new PropertyDef(typeof(IRootElement),
                typeof(IRootElement).GetProperty("ChildDictionary", BindingFlags.Instance | BindingFlags.Public));
            ChildConfigurationCollectionPropertyDef = new PropertyDef(typeof(IRootElement),
                typeof(IRootElement).GetProperty("ChildCollection", BindingFlags.Instance | BindingFlags.Public));
            ReadOnlyChildConfigurationDictionaryPropertyDef = new PropertyDef(typeof(IRootElement),
                typeof(IRootElement).GetProperty("ReadOnlyChildDictionary", BindingFlags.Instance | BindingFlags.Public));
            ReadOnlyChildConfigurationCollectionPropertyDef = new PropertyDef(typeof(IRootElement),
                typeof(IRootElement).GetProperty("ReadOnlyChildCollection", BindingFlags.Instance | BindingFlags.Public));

            Data = new[]
            {
                GetConfigurationObject("a", 0, false), GetConfigurationObject("b", 1, false), GetConfigurationObject("c", 2, false),
                GetConfigurationObject("d", 3, false), GetConfigurationObject("e", 4, true)
            };
        }

        public PropertyDef ChildConfigurationCollectionPropertyDef { get; }

        public PropertyDef ChildConfigurationDictionaryPropertyDef { get; }

        public PropertyDef ReadOnlyChildConfigurationCollectionPropertyDef { get; }

        public PropertyDef ReadOnlyChildConfigurationDictionaryPropertyDef { get; }

        private PropertyDef BooleanPropertyDef { get; }

        private IChildElement[] Data { get; }

        public TestDataContext GetContext()
        {
            return new TestDataContext(this);
        }

        private IChildElement GetConfigurationObject(string name, int value, bool isDirty)
        {
            var mock = new Mock<IChildElement>();
            mock.Setup(x => x.PropertyDef).Returns(new PropertyDef(typeof(IDummyInterface),
                typeof(IDummyInterface).GetProperty(name, BindingFlags.Instance | BindingFlags.Public)));
            mock.Setup(x => x.IsDirty).Returns(isDirty);
            mock.Setup(x => x.Name).Returns(name);
            mock.Setup(x => x.Value).Returns(value);
            return mock.Object;
        }

        public sealed class TestDataContext : Disposable
        {
            private readonly TestDataFixture _propertyTestData;

            public TestDataContext(TestDataFixture propertyTestData)
            {
                _propertyTestData = propertyTestData;

                BooleanPropertyDef = propertyTestData.BooleanPropertyDef;
                ChildConfigurationCollectionPropertyDef = propertyTestData.ChildConfigurationCollectionPropertyDef;
                ChildConfigurationDictionaryPropertyDef = propertyTestData.ChildConfigurationDictionaryPropertyDef;
                ReadOnlyChildConfigurationCollectionPropertyDef = propertyTestData.ReadOnlyChildConfigurationCollectionPropertyDef;
                ReadOnlyChildConfigurationDictionaryPropertyDef = propertyTestData.ReadOnlyChildConfigurationDictionaryPropertyDef;

                ChildDictionary = new Dictionary<string, IChildElement>()
                {
                    { propertyTestData.Data[0].PropertyDef.PropertyName, propertyTestData.Data[0] },
                    { propertyTestData.Data[1].PropertyDef.PropertyName, propertyTestData.Data[1] },
                    { propertyTestData.Data[2].PropertyDef.PropertyName, propertyTestData.Data[2] },
                    { propertyTestData.Data[3].PropertyDef.PropertyName, propertyTestData.Data[3] },
                    { propertyTestData.Data[4].PropertyDef.PropertyName, propertyTestData.Data[4] }
                };

                Configuration = new ConfigurationFixture();
            }

            public PropertyDef BooleanPropertyDef { get; }

            public PropertyDef ChildConfigurationCollectionPropertyDef { get; }

            public PropertyDef ChildConfigurationDictionaryPropertyDef { get; }

            public Dictionary<string, IChildElement> ChildDictionary { get; }

            public ConfigurationFixture Configuration { get; }

            public PropertyDef ReadOnlyChildConfigurationCollectionPropertyDef { get; }

            public PropertyDef ReadOnlyChildConfigurationDictionaryPropertyDef { get; }

            public IChildElement GetChildElement(string name)
            {
                return ChildDictionary[name];
            }

            public IChildElement GetConfigurationObject(string name, int value, bool isDirty)
            {
                return _propertyTestData.GetConfigurationObject(name, value, isDirty);
            }
        }
    }
}