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
 * Copyright © 2019 Jonathan Evans (jevans@open-collar.org.uk).
 */

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public interface IRootElement : IConfigurationObject
    {
        // We would expect this to be implemented as a read-only property.
        bool BooleanPropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        bool BooleanPropertyB
        {
            get; set;
        }

        // We would expect this to be implemented as a read-only property.
        char CharPropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        char CharPropertyB
        {
            get; set;
        }

        // TODO: THIS SHOULD NOT BE NULLABLE
        IConfigurationCollection<IChildElement>? ChildElements
        {
            get;
        }

        // We would expect this to be implemented as a read-only property.
        System.DateTimeOffset DateTimeOffsetPropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        System.DateTimeOffset DateTimeOffsetPropertyB
        {
            get; set;
        }

        // We would expect this to be implemented as a read-only property.
        System.DateTime DateTimePropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        System.DateTime DateTimePropertyB
        {
            get; set;
        }

        // We would expect this to be implemented as a read-only property.
        double DoublePropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        double DoublePropertyB
        {
            get; set;
        }

        // We would expect this to be implemented as a read-only property.
        System.Reflection.BindingFlags EnumPropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        System.Reflection.BindingFlags EnumPropertyB
        {
            get; set;
        }

        // We would expect this to be implemented as a read-only property.
        short Int16PropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        short Int16PropertyB
        {
            get; set;
        }

        // We would expect this to be implemented as a read-only property.
        int Int32PropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        int Int32PropertyB
        {
            get; set;
        }

        // We would expect this to be implemented as a read-only property.
        long Int64PropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        long Int64PropertyB
        {
            get; set;
        }

        // We would expect this to be implemented as a read-only property.
        sbyte SBytePropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        sbyte SBytePropertyB
        {
            get; set;
        }

        // We would expect this to be implemented as a read-only property.
        float SinglePropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        float SinglePropertyB
        {
            get; set;
        }

        // We would expect this to be implemented as a read-only property.
        string StringPropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        string StringPropertyB
        {
            get; set;
        }

        // We would expect this to be implemented as a read/write property.
        string StringPropertyC
        {
            get; set;
        }

        // We would expect this to be implemented as a read-only property.
        System.TimeSpan TimeSpanPropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        System.TimeSpan TimeSpanPropertyB
        {
            get; set;
        }
    }
}