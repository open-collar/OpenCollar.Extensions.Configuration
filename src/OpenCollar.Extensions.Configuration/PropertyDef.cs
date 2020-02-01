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
    ///     Defines the way in which the value returned by a property is implemented.
    /// </summary>
    internal enum ImplementationKind
    {
        /// <summary>
        ///     The implementation is unknown or undefined.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     The implementation is the naive type (i.e. nothing special is required).
        /// </summary>
        Naive,

        /// <summary>
        ///     The implementation is derived from <see cref="ConfigurationObjectBase{TInterface}" />.
        /// </summary>
        ConfigurationObject,

        /// <summary>
        ///     The implementation is derived from <see cref="ConfigurationCollection{TInterface}" />.
        /// </summary>
        ConfigurationCollection,

        /// <summary>
        ///     The implementation is derived from <see cref="ConfigurationDictionary{TInterface}" />.
        /// </summary>
        ConfigurationDictionary
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
        /// <param name="context"> The context in which the property is being defined. </param>
        internal PropertyDef(string path, Type interfaceType, PropertyInfo propertyInfo, object? defaultValue, ConfigurationContext context) : this(path, interfaceType, propertyInfo, context)
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
        /// <param name="context"> The context in which the property is being defined. </param>
        internal PropertyDef(string path, Type interfaceType, PropertyInfo propertyInfo, ConfigurationContext context)
        {
            Path = path;
            PropertyName = propertyInfo.Name;
            Type = propertyInfo.PropertyType;
            UnderlyingType = GetUnderlyingType(propertyInfo.PropertyType);
            IsNullable = PropertyIsNullable(interfaceType, propertyInfo);
            IsReadOnly = !propertyInfo.CanWrite;
            ImplementationKind = GetImplementationKind(UnderlyingType);

            Type elementType;

            switch(ImplementationKind)
            {
                case ImplementationKind.ConfigurationObject:
                    elementType = UnderlyingType;
                    ImplementationType = ServiceCollectionExtensions.GenerateConfigurationObjectType(elementType, context);
                    ElementType = null;
                    break;

                case ImplementationKind.ConfigurationCollection:
                    elementType = UnderlyingType.GenericTypeArguments.First();
                    if(IsReadOnly)
                    {
                        ImplementationType = typeof(ReadOnlyConfigurationCollection<>).MakeGenericType(new[] { elementType });
                    }
                    else
                    {
                        ImplementationType = typeof(ConfigurationCollection<>).MakeGenericType(new[] { elementType });
                    }
                    ElementType = elementType;
                    break;

                case ImplementationKind.ConfigurationDictionary:
                    elementType = UnderlyingType.GenericTypeArguments.First();
                    if(IsReadOnly)
                    {
                        ImplementationType = typeof(ReadOnlyConfigurationDictionary<>).MakeGenericType(new[] { elementType });
                    }
                    else
                    {
                        ImplementationType = typeof(ConfigurationDictionary<>).MakeGenericType(new[] { elementType });
                    }
                    ElementType = elementType;
                    break;

                default:
                    ImplementationType = null;
                    ElementType = null;
                    break;
            }
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
        ///     Gets the type of the elements in an array or dictionary.
        /// </summary>
        public Type? ElementType
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
        ///     Gets the type to use if generating an instance of the property represented.
        /// </summary>
        public Type? ImplementationType
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
        ///     Gets the kind of the implementation required for the value returned by the property defined.
        /// </summary>
        /// <value> The kind of the implementation required for the value returned by the property defined. </value>
        internal ImplementationKind ImplementationKind
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

        /// <summary>
        ///     Determines whether a property can be set to <see langword="null" />.
        /// </summary>
        /// <param name="implementingType"> Type of the object to which the property belongs. </param>
        /// <param name="property"> The definition of the property to examine. </param>
        /// <returns> </returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="implementingType" /> must be the type which defines property.
        /// </exception>
        public static bool PropertyIsNullable(Type implementingType, PropertyInfo property)
        {
            if(!implementingType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).Contains(property))
                throw new ArgumentException("'enclosingType' must be the type which defines property.");

            var nullable = property.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == @"System.Runtime.CompilerServices.NullableAttribute");
            if(!ReferenceEquals(nullable, null) && (nullable.ConstructorArguments.Count == 1))
            {
                var attributeArgument = nullable.ConstructorArguments[0];
                if(attributeArgument.ArgumentType == typeof(byte[]))
                {
                    var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value;
                    if((args.Count > 0) && (args[0].ArgumentType == typeof(byte)))
                    {
                        return (byte)args[0].Value == 2;
                    }
                }
                else if(attributeArgument.ArgumentType == typeof(byte))
                {
                    return (byte)attributeArgument.Value == 2;
                }
            }

            var context = implementingType.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
            if(!ReferenceEquals(context, null) &&
                (context.ConstructorArguments.Count == 1) &&
                (context.ConstructorArguments[0].ArgumentType == typeof(byte)))
            {
                if((byte)context.ConstructorArguments[0].Value == 2)
                {
                    return true;
                }
            }

            // Couldn't find a suitable attribute
            return property.PropertyType.IsConstructedGenericType && (property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        ///     Determines whether the current value is the same as the original value.
        /// </summary>
        /// <param name="original"> The original value. </param>
        /// <param name="current"> The current value. </param>
        /// <returns> <see langword="true" /> if the values are the same; otherwise, <see langword="false" />. </returns>
        internal bool AreEqual(object? original, object? current)
        {
            if(ReferenceEquals(original, current))
            {
                return true;
            }

            if(ReferenceEquals(original, null) || ReferenceEquals(current, null))
            {
                return false;
            }

            var configurationObject = current as IConfigurationObject;
            if(!ReferenceEquals(configurationObject, null) && configurationObject.IsDirty)
            {
                return true;
            }

            return original.Equals(current);
        }

        /// <summary>
        ///     Parses a string value into the type defined by the property definition.
        /// </summary>
        /// <param name="stringRepresentation"> The string to parse. </param>
        /// <returns> The string parsed as the type of this property. </returns>
        /// <exception cref="ConfigurationException"> Value could not be converted. </exception>
        internal object? ConvertStringToValue(string? stringRepresentation)
        {
            if(ReferenceEquals(stringRepresentation, null))
            {
                if(IsNullable)
                {
                    return DefaultValue;
                }
                else
                {
                    throw new ConfigurationException(Path, $"Null value cannot be assigned to configuration path: '{Path}'.");
                }
            }

            var type = UnderlyingType;

            if(type.IsEnum)
            {
                return Enum.Parse(type, stringRepresentation);
            }

            if(type == typeof(string))
            {
                return stringRepresentation;
            }

            if(type == typeof(char))
            {
                if(stringRepresentation.Length != 1)
                {
                    throw new ConfigurationException(Path, $"Value could not be treated as a 'char'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
                }
                return stringRepresentation[0];
            }

            if(type == typeof(short))
            {
                if(short.TryParse(stringRepresentation, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(Path, $"Value could not be parsed as an 'Int16'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(sbyte))
            {
                if(sbyte.TryParse(stringRepresentation, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(Path, $"Value could not be parsed as an 'SByte'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(int))
            {
                if(int.TryParse(stringRepresentation, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var int32Value))
                {
                    return int32Value;
                }
                throw new ConfigurationException(Path, $"Value could not be parsed as an 'Int32'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(long))
            {
                if(long.TryParse(stringRepresentation, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var int64Value))
                {
                    return int64Value;
                }
                throw new ConfigurationException(Path, $"Value could not be parsed as an 'Int64'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(float))
            {
                if(float.TryParse(stringRepresentation, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(Path, $"Value could not be parsed as an 'Single'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(double))
            {
                if(double.TryParse(stringRepresentation, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(Path, $"Value could not be parsed as an 'Double'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(decimal))
            {
                if(decimal.TryParse(stringRepresentation, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(Path, $"Value could not be parsed as an 'Decimal'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(DateTime))
            {
                if(System.DateTime.TryParse(stringRepresentation, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(Path, $"Value could not be parsed as an 'DateTime'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(DateTimeOffset))
            {
                if(System.DateTimeOffset.TryParse(stringRepresentation, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(Path, $"Value could not be parsed as an 'DateTimeOffset'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(TimeSpan))
            {
                if(System.TimeSpan.TryParse(stringRepresentation, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(Path, $"Value could not be parsed as an 'TimeSpan'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(bool))
            {
                if(bool.TryParse(stringRepresentation, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(Path, $"Value could not be parsed as an 'Boolean'; configuration path: '{Path}'; value: '{stringRepresentation}'.");
            }

            return Convert.ChangeType(stringRepresentation, Type);
        }

        /// <summary>
        ///     Given a value that can be assigned to the property represented, returns a string equalivalent.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns> The string equivalent of the value given. </returns>
        internal string? ConvertValueToString(object? value)
        {
            if(ReferenceEquals(value, null))
            {
                return null;
            }

            var type = UnderlyingType;

            if(type.IsEnum)
            {
                return Enum.GetName(type, value);
            }

            if(type == typeof(string))
            {
                return (string)value;
            }

            if(type == typeof(char))
            {
                return new string(((char)value), 1);
            }

            if(type == typeof(short))
            {
                return ((short)value).ToString("D", System.Globalization.CultureInfo.InvariantCulture);
            }

            if(type == typeof(sbyte))
            {
                return ((sbyte)value).ToString("D", System.Globalization.CultureInfo.InvariantCulture);
            }

            if(type == typeof(int))
            {
                return ((int)value).ToString("D", System.Globalization.CultureInfo.InvariantCulture);
            }

            if(type == typeof(long))
            {
                return ((long)value).ToString("D", System.Globalization.CultureInfo.InvariantCulture);
            }

            if(type == typeof(float))
            {
                return ((float)value).ToString("G9", System.Globalization.CultureInfo.InvariantCulture);
            }

            if(type == typeof(double))
            {
                return ((double)value).ToString("G17", System.Globalization.CultureInfo.InvariantCulture);
            }

            if(type == typeof(decimal))
            {
                return ((decimal)value).ToString("G", System.Globalization.CultureInfo.InvariantCulture);
            }

            if(type == typeof(DateTime))
            {
                return ((DateTime)value).ToString("O", System.Globalization.CultureInfo.InvariantCulture);
            }

            if(type == typeof(DateTimeOffset))
            {
                return ((DateTimeOffset)value).ToString("O", System.Globalization.CultureInfo.InvariantCulture);
            }

            if(type == typeof(TimeSpan))
            {
                return ((TimeSpan)value).ToString("c", System.Globalization.CultureInfo.InvariantCulture);
            }

            if(type == typeof(bool))
            {
                return ((bool)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            // For anything else, let's hope that "ToString" is good enough.
            return value.ToString();
        }

        /// <summary>
        ///     Gets the kind of the implementation required for the type given.
        /// </summary>
        /// <param name="type"> The type for which the implementation kind is required. </param>
        /// <returns> </returns>
        private static ImplementationKind GetImplementationKind(Type type)
        {
            if(IsConfigurationDictionary(type))
            {
                return ImplementationKind.ConfigurationDictionary;
            }

            if(IsConfigurationCollection(type))
            {
                return ImplementationKind.ConfigurationCollection;
            }

            if(typeof(IConfigurationObject).IsAssignableFrom(type))
            {
                return ImplementationKind.ConfigurationObject;
            }

            return ImplementationKind.Naive;
        }

        /// <summary>
        ///     Determines whether the specified type is a is configuration collection.
        /// </summary>
        /// <param name="type"> The type to verify. </param>
        /// <returns> <see langword="true" /> if the type is configuration collection; otherwise, <see langword="false" />. </returns>
        private static bool IsConfigurationCollection(Type type)
        {
            if(!type.IsConstructedGenericType)
            {
                return false;
            }

            if(type.GetGenericTypeDefinition() != typeof(IConfigurationCollection<>))
            {
                return false;
            }

            var arguments = type.GetGenericArguments();

            if(arguments.Length != 1)
            {
                return false;
            }

            return typeof(IConfigurationObject).IsAssignableFrom(arguments[0]);
        }

        /// <summary>
        ///     Determines whether the specified type is a is configuration dictionary.
        /// </summary>
        /// <param name="type"> The type to verify. </param>
        /// <returns> <see langword="true" /> if the type is configuration dictionary; otherwise, <see langword="false" />. </returns>
        private static bool IsConfigurationDictionary(Type type)
        {
            if(!type.IsConstructedGenericType)
            {
                return false;
            }

            if(type.GetGenericTypeDefinition() != typeof(IConfigurationDictionary<>))
            {
                return false;
            }

            var arguments = type.GetGenericArguments();

            if(arguments.Length != 1)
            {
                return false;
            }

            return typeof(IConfigurationObject).IsAssignableFrom(arguments[0]);
        }
    }
}