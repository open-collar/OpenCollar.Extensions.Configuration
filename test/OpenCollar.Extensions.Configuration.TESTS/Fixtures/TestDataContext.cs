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

namespace OpenCollar.Extensions.Configuration.TESTS.Collections
{
    public sealed partial class TestDataFixture
    {
        internal sealed class TestDataContext : Disposable
        {
            private readonly TestDataFixture _propertyTestData;

            internal TestDataContext(TestDataFixture propertyTestData)
            {
                _propertyTestData = propertyTestData;

                BooleanPropertyDef = propertyTestData.BooleanPropertyDef;
                ChildConfigurationCollectionPropertyDef = propertyTestData.ChildConfigurationCollectionPropertyDef;
                ChildConfigurationDictionaryPropertyDef = propertyTestData.ChildConfigurationDictionaryPropertyDef;
                ReadOnlyChildConfigurationCollectionPropertyDef = propertyTestData.ReadOnlyChildConfigurationCollectionPropertyDef;
                ReadOnlyChildConfigurationDictionaryPropertyDef = propertyTestData.ReadOnlyChildConfigurationDictionaryPropertyDef;

                ChildDictionary = new Dictionary<string, IChildElement>()
                {
                { propertyTestData.Data[0].PropertyDef().PropertyName, propertyTestData.Data[0] },
                { propertyTestData.Data[1].PropertyDef().PropertyName, propertyTestData.Data[1] },
                { propertyTestData.Data[2].PropertyDef().PropertyName, propertyTestData.Data[2] },
                { propertyTestData.Data[3].PropertyDef().PropertyName, propertyTestData.Data[3] },
                { propertyTestData.Data[4].PropertyDef().PropertyName, propertyTestData.Data[4] }
                };

                Configuration = new ConfigurationFixture();
            }

            internal IPropertyDef BooleanPropertyDef { get; }

            internal IPropertyDef ChildConfigurationCollectionPropertyDef { get; }

            internal IPropertyDef ChildConfigurationDictionaryPropertyDef { get; }

            internal Dictionary<string, IChildElement> ChildDictionary { get; }

            internal ConfigurationFixture Configuration { get; }

            internal IPropertyDef ReadOnlyChildConfigurationCollectionPropertyDef { get; }

            internal IPropertyDef ReadOnlyChildConfigurationDictionaryPropertyDef { get; }

            internal IChildElement GetChildElement(string name)
            {
                return ChildDictionary[name];
            }

            internal IChildElement GetConfigurationObject(string name, int value, bool isDirty)
            {
                return _propertyTestData.GetConfigurationObject(name, value, isDirty);
            }
        }
    }
}