﻿/*
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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Specifies the ways in which a property can be used to represent structural elements.
    /// </summary>
    public enum StructuralElementKind
    {
        /// <summary>
        ///     The kind of structural element is unknown or undefined. Use of this value will usually result in an
        ///     error; it is provided to as sentinel to detect accidental usages.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     The element is a property that contains a single, unstructured, value.
        /// </summary>
        Property,

        /// <summary>
        ///     The property contains an array of elements.
        /// </summary>
        Array,

        /// <summary>
        ///     The property contains a dictionary of elements.
        /// </summary>
        Dictionary
    }

    /// <summary>
    ///     The definition of a property of a configuration object.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Path}")]
    public class PropertyDef
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyDef" /> class.
        /// </summary>
        /// <param name="path"> The colon-delimited path to the underlying configuration value. </param>
        /// <param name="interfaceType"> The type of the interface from which the property is taken. </param>
        /// <param name="propertyInfo"> The definition of the property. </param>
        /// <param name="defaultValue"> The default value. </param>
        internal PropertyDef(string path, Type interfaceType, PropertyInfo propertyInfo, object? defaultValue) : this(path, interfaceType, propertyInfo)
        {
            HasDefaultValue = true;
            DefaultValue = defaultValue;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyDef" /> class.
        /// </summary>
        /// <param name="path"> The colon-delimited path to the underlying configuration value. </param>
        /// <param name="interfaceType"> The type of the interface from which the property is taken. </param>
        /// <param name="propertyInfo"> The definition of the property. </param>
        internal PropertyDef(string path, Type interfaceType, PropertyInfo propertyInfo)
        {
            Path = path;
            PropertyName = propertyInfo.Name;
            Type = propertyInfo.PropertyType;
            UnderlyingType = GetUnderlyingType(propertyInfo.PropertyType);
            IsNullable = TypeIsNullable(interfaceType, propertyInfo);
            IsReadOnly = !propertyInfo.CanWrite;
        }

        /// <summary>
        ///     Gets or sets the default value.
        /// </summary>
        /// <value> The default value. Can be <see langword="null" />. </value>
        public object? DefaultValue
        {
            get;
        }

        /// <summary>
        ///     Gets a value indicating whether the property represented by this instance has default a value.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the property represented by this instance has default a value; otherwise, <see langword="false" />.
        /// </value>
        public bool HasDefaultValue
        {
            get;
        }

        /// <summary>
        ///     Gets a value indicating whether the value of the property represented by this instance can be <see langword="null" />.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the value of the property represented by this instance is nullable;
        ///     otherwise, <see langword="false" />.
        /// </value>
        public bool IsNullable
        {
            get;
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the property is read only; otherwise, <see langword="false" /> for an
        ///     editable property.
        /// </value>
        public bool IsReadOnly
        {
            get;
        }

        /// <summary>
        ///     Gets the colon-delimited path to the underlying configuration value.
        /// </summary>
        /// <value> The colon-delimited path to the underlying configuration value. </value>
        public string Path
        {
            get;
        }

        /// <summary>
        ///     Gets the name of the property represented by this object.
        /// </summary>
        /// <value> The name of the property represented by this object. </value>
        public string PropertyName
        {
            get;
        }

        /// <summary>
        ///     Gets the type of the value held in the property.
        /// </summary>
        /// <value> The type of the value held in the property. </value>
        public Type Type
        {
            get;
        }

        /// <summary>
        ///     Gets the basic type represented by the type of the property (for example by <see cref="int?" /> would
        ///     have an underlying type of <see cref="int" />).
        /// </summary>
        /// <returns> The basic type represented by the type given. </returns>
        public Type UnderlyingType
        {
            get;
        }

        /// <summary>
        ///     Gets the basic type represented by the type given.
        /// </summary>
        /// <param name="type"> The type for which to find the underlying type. </param>
        /// <returns> </returns>
        public static Type GetUnderlyingType(Type type)
        {
            if(type.IsConstructedGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                type = type.GetGenericArguments()[0];
            }

            return type;
        }

        public static bool TypeIsNullable(Type enclosingType, PropertyInfo property)
        {
            if(!enclosingType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).Contains(property))
                throw new ArgumentException("enclosingType must be the type which defines property");

            var nullable = property.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
            if(nullable != null && nullable.ConstructorArguments.Count == 1)
            {
                var attributeArgument = nullable.ConstructorArguments[0];
                if(attributeArgument.ArgumentType == typeof(byte[]))
                {
                    var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value;
                    if(args.Count > 0 && args[0].ArgumentType == typeof(byte))
                    {
                        return (byte)args[0].Value == 2;
                    }
                }
                else if(attributeArgument.ArgumentType == typeof(byte))
                {
                    return (byte)attributeArgument.Value == 2;
                }
            }

            var context = enclosingType.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
            if(context != null &&
                context.ConstructorArguments.Count == 1 &&
                context.ConstructorArguments[0].ArgumentType == typeof(byte))
            {
                if((byte)context.ConstructorArguments[0].Value == 2)
                {
                    return true;
                }
            }

            // Couldn't find a suitable attribute
            return property.PropertyType.IsConstructedGenericType && (property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}