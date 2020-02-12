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
using System.ComponentModel;
using System.Reflection;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class ChildElementMockMultiInterface : IChildElement, IRootElement
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool BooleanPropertyA
        {
            get;
        }

        public bool BooleanPropertyB
        {
            get; set;
        }

        public byte BytePropertyA
        {
            get;
        }

        public byte BytePropertyB
        {
            get;
            set;
        }

        public char CharPropertyA
        {
            get;
        }

        public char CharPropertyB
        {
            get; set;
        }

        public IConfigurationCollection<IChildElement> ChildCollection
        {
            get; set;
        }

        public IConfigurationDictionary<IChildElement> ChildDictionary
        {
            get; set;
        }

        public string CustomProperty
        {
            get; set;
        }

        public DateTimeOffset DateTimeOffsetPropertyA
        {
            get;
        }

        public DateTimeOffset DateTimeOffsetPropertyB
        {
            get; set;
        }

        public DateTime DateTimePropertyA
        {
            get;
        }

        public DateTime DateTimePropertyB
        {
            get; set;
        }

        public decimal DecimalPropertyA
        {
            get;
        }

        public decimal DecimalPropertyB
        {
            get; set;
        }

        public double DoublePropertyA
        {
            get;
        }

        public double DoublePropertyB
        {
            get; set;
        }

        public BindingFlags EnumPropertyA
        {
            get;
        }

        public BindingFlags EnumPropertyB
        {
            get; set;
        }

        public short Int16PropertyA
        {
            get;
        }

        public short Int16PropertyB
        {
            get; set;
        }

        public int Int32PropertyA
        {
            get;
        }

        public int Int32PropertyB
        {
            get; set;
        }

        public int Int32PropertyC
        {
            get;
        }

        public int Int32PropertyD
        {
            get;
        }

        public long Int64PropertyA
        {
            get;
        }

        public long Int64PropertyB
        {
            get; set;
        }

        public bool IsDirty
        {
            get; set;
        }

        public bool IsReadOnly
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public NonFlagsEnum NonFlagsEnumPropertyA
        {
            get;
        }

        public NonFlagsEnum NonFlagsEnumPropertyB
        {
            get;
            set;
        }

        public PropertyDef PropertyDef
        {
            get; set;
        }

        public IConfigurationCollection<IChildElement> ReadOnlyChildCollection
        {
            get;
        }

        public IConfigurationDictionary<IChildElement> ReadOnlyChildDictionary
        {
            get;
        }

        public sbyte SBytePropertyA
        {
            get;
        }

        public sbyte SBytePropertyB
        {
            get; set;
        }

        public float SinglePropertyA
        {
            get;
        }

        public float SinglePropertyB
        {
            get; set;
        }

        public float SinglePropertyNoDefault
        {
            get; set;
        }

        public float? SinglePropertyWithDefault
        {
            get; set;
        }

        public string StringPropertyA
        {
            get;
        }

        public string StringPropertyB
        {
            get; set;
        }

        public string StringPropertyC
        {
            get; set;
        }

        public TimeSpan TimeSpanPropertyA
        {
            get;
        }

        public TimeSpan TimeSpanPropertyB
        {
            get; set;
        }

        public int Value
        {
            get; set;
        }

        public void Delete() => throw new NotImplementedException();

        public void Dispose() => throw new NotImplementedException();

        public string GetPath() => throw new NotImplementedException();

        public void Load() => throw new NotImplementedException();

        public void Save() => throw new NotImplementedException();
    }
}