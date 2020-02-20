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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     The definition of a property of a configuration object.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/PropertyDef/PropertyDef.svg" />
    /// </remarks>
    [DebuggerDisplay("PropertyDef[{" + nameof(PropertyName) + "}]")]
    internal class PropertyDef : IPropertyDef
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyDef" /> class.
        /// </summary>
        /// <param name="propertyInfo"> The definition of the property. </param>
        /// <exception cref="InvalidPropertyException">
        ///     'ConfigurationValue' attribute on property specifies that persistence should only save values (not
        ///     load), but no default value is provided.
        /// </exception>
        internal PropertyDef(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            PropertyName = propertyInfo.Name;
            Type = propertyInfo.PropertyType;
            UnderlyingType = GetUnderlyingType(propertyInfo.PropertyType);
            IsReadOnly = !propertyInfo.CanWrite;

            var pathAttributes = propertyInfo.GetCustomAttributes(typeof(PathAttribute), true);
            if(pathAttributes.Length > 0)
            {
                var pathAttribute = ((PathAttribute)pathAttributes[0]);

                PathSection = pathAttribute.Path;
                PathModifier = pathAttribute.Usage;
            }
            else
            {
                PathModifier = PathIs.Suffix;
                PathSection = PropertyName;
            }

            Implementation = new Implementation(UnderlyingType);
            switch(Implementation.ImplementationKind)
            {
                case ImplementationKind.ConfigurationCollection:
                case ImplementationKind.ConfigurationDictionary:
                case ImplementationKind.ConfigurationObject:
                    ElementImplementation = new Implementation(Implementation.Type);
                    break;
            }

            var configurationAttributes = propertyInfo.GetCustomAttributes<ConfigurationAttribute>(true).ToList();
            if(configurationAttributes.Count > 0)
            {
                var attribute = configurationAttributes[0];
                DefaultValue = attribute.DefaultValue;
                HasDefaultValue = attribute.IsDefaultValueSet;
                Persistence = attribute.Persistence;
            }
            else
            {
                Persistence = ConfigurationPersistenceActions.LoadAndSave;
            }

            if(!Persistence.HasFlag(ConfigurationPersistenceActions.LoadOnly) && !HasDefaultValue)
            {
                var propertyName = $"[{propertyInfo.DeclaringType.Namespace}.{propertyInfo.DeclaringType.Name}].{propertyInfo.Name}";
                throw new InvalidPropertyException(propertyName,
                $"'ConfigurationValue' attribute on property \"{propertyName}\" specifies that persistence should only save or ignore values (not load), but no default value is provided.");
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
        ///     Gets the details of the specific implementation of this property.
        /// </summary>
        /// <value> The details of the specific implementation of this property. </value>
        public IImplementation? ElementImplementation
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
        ///     Gets the details of the specific implementation of this property.
        /// </summary>
        /// <value> The details of the specific implementation of this property. </value>
        public IImplementation Implementation
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
        ///     Gets the path modifier.
        /// </summary>
        /// <value> The path modifier. </value>
        public PathIs PathModifier
        {
            get;
        }

        /// <summary>
        ///     Gets the path section.
        /// </summary>
        /// <value> The path section. </value>
        public string PathSection
        {
            get;
        }

        public ConfigurationPersistenceActions Persistence
        {
            get;
        }

        /// <summary>
        ///     Gets the property information that defines the interface property.
        /// </summary>
        /// <value> The property information that defines the interface property. </value>
        public PropertyInfo PropertyInfo
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
        ///     Gets the basic type represented by the type of the property (for example by <c> int? </c> would have an
        ///     underlying type of <see cref="int" /> ).
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

        /// <summary>
        ///     Gets the path to this configuration object.
        /// </summary>
        /// <returns> A string containing the path to this configuration object. </returns>
        public string CalculatePath(IConfigurationParent? parent)
        {
            if(ReferenceEquals(parent, null))
            {
                return PathSection;
            }

            switch(PathModifier)
            {
                case PathIs.Suffix:
                    return PathHelper.CalculatePath(parent.CalculatePath(), PathSection);
            }

            return PathSection;
        }

        /// <summary>
        ///     Parses a string value into the type defined by the property definition.
        /// </summary>
        /// <param name="path"> The path to the value being converted (used in error messages). </param>
        /// <param name="stringRepresentation"> The string to parse. </param>
        /// <returns> The string parsed as the type of this property. </returns>
        /// <exception cref="ConfigurationException"> Value could not be converted. </exception>
        public object? ConvertStringToValue(string path, string? stringRepresentation)
        {
            if(ReferenceEquals(stringRepresentation, null))
            {
                if(HasDefaultValue)
                {
                    return DefaultValue;
                }
                else
                {
                    throw new ConfigurationException(path, $"Null value cannot be assigned to configuration path: '{path}'.");
                }
            }

            var type = UnderlyingType;

            if(type.IsEnum)
            {
                if(Enum.TryParse(type, stringRepresentation, out var enumValue))
                {
                    if(type.GetCustomAttributes<FlagsAttribute>().Any())
                    {
                        return enumValue;
                    }

                    if(!Enum.IsDefined(type, enumValue))
                    {
                        throw new ConfigurationException(path,
                        $"Value could not be treated as a '{type.FullName}'; configuration path: '{path}'; value: '{stringRepresentation}'.");
                    }

                    return enumValue;
                }

                throw new ConfigurationException(path,
                $"Value could not be treated as a '{type.FullName}'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(string))
            {
                return stringRepresentation;
            }

            if(type == typeof(char))
            {
                if(stringRepresentation.Length != 1)
                {
                    throw new ConfigurationException(path,
                    $"Value could not be treated as a 'char'; configuration path: '{path}'; value: '{stringRepresentation}'.");
                }

                return stringRepresentation[0];
            }

            if(type == typeof(short))
            {
                if(short.TryParse(stringRepresentation, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'Int16'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(sbyte))
            {
                if(sbyte.TryParse(stringRepresentation, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'SByte'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(byte))
            {
                if(byte.TryParse(stringRepresentation, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'Byte'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(int))
            {
                if(int.TryParse(stringRepresentation, NumberStyles.Integer, CultureInfo.InvariantCulture, out var int32Value))
                {
                    return int32Value;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'Int32'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(long))
            {
                if(long.TryParse(stringRepresentation, NumberStyles.Integer, CultureInfo.InvariantCulture, out var int64Value))
                {
                    return int64Value;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'Int64'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(float))
            {
                if(float.TryParse(stringRepresentation, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'Single'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(double))
            {
                if(double.TryParse(stringRepresentation, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'Double'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(decimal))
            {
                if(decimal.TryParse(stringRepresentation, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'Decimal'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(DateTime))
            {
                if(DateTime.TryParse(stringRepresentation, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
                {
                    return parsed;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'DateTime'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(DateTimeOffset))
            {
                if(DateTimeOffset.TryParse(stringRepresentation, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
                {
                    return parsed;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'DateTimeOffset'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(TimeSpan))
            {
                if(TimeSpan.TryParse(stringRepresentation, CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'TimeSpan'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            if(type == typeof(bool))
            {
                if(bool.TryParse(stringRepresentation, out var parsed))
                {
                    return parsed;
                }

                throw new ConfigurationException(path,
                $"Value could not be parsed as an 'Boolean'; configuration path: '{path}'; value: '{stringRepresentation}'.");
            }

            return Convert.ChangeType(stringRepresentation, Type, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Given a value that can be assigned to the property represented, returns a string equivalent.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns> The string equivalent of the value given. </returns>
        public string? ConvertValueToString(object? value)
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
                return ((short)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if(type == typeof(sbyte))
            {
                return ((sbyte)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if(type == typeof(byte))
            {
                return ((byte)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if(type == typeof(int))
            {
                return ((int)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if(type == typeof(long))
            {
                return ((long)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if(type == typeof(float))
            {
                return ((float)value).ToString("G9", CultureInfo.InvariantCulture);
            }

            if(type == typeof(double))
            {
                return ((double)value).ToString("G17", CultureInfo.InvariantCulture);
            }

            if(type == typeof(decimal))
            {
                return ((decimal)value).ToString("G", CultureInfo.InvariantCulture);
            }

            if(type == typeof(DateTime))
            {
                return ((DateTime)value).ToString("O", CultureInfo.InvariantCulture);
            }

            if(type == typeof(DateTimeOffset))
            {
                return ((DateTimeOffset)value).ToString("O", CultureInfo.InvariantCulture);
            }

            if(type == typeof(TimeSpan))
            {
                return ((TimeSpan)value).ToString("c", CultureInfo.InvariantCulture);
            }

            if(type == typeof(bool))
            {
                return ((bool)value).ToString(CultureInfo.InvariantCulture);
            }

            // For anything else, let's hope that "ToString" is good enough.
            return value.ToString();
        }

        /// <summary>
        ///     Copies the value of an element.
        /// </summary>
        /// <typeparam name="TElement"> The type of the element to copy. </typeparam>
        /// <param name="implementation"> The details of the implementation for which to make a copy. </param>
        /// <param name="value"> The value to copy. </param>
        /// <param name="parent"> The parent object to which cloned configuration objects will belong. </param>
        /// <param name="configurationRoot">
        ///     The configuration root from which cloned configuration objects are to be populated.
        /// </param>
        /// <returns> The newly copied element. </returns>
        public TElement CopyValue<TElement>(IImplementation implementation, TElement value, IConfigurationParent parent, IConfigurationRoot configurationRoot)
        {
            if(ReferenceEquals(value, null))
            {
                return default!;
            }

            if(ReferenceEquals(ElementImplementation, null))
            {
                return value;
            }

            switch(implementation.ImplementationKind)
            {
                case ImplementationKind.ConfigurationCollection:
                    return (TElement)Activator.CreateInstance(implementation.ImplementationType, parent, this, configurationRoot, value);

                case ImplementationKind.ConfigurationDictionary:
                    return (TElement)Activator.CreateInstance(implementation.ImplementationType, parent, this, configurationRoot, value);

                case ImplementationKind.ConfigurationObject:
                    var clone = Activator.CreateInstance(implementation.ImplementationType, this, configurationRoot, parent);
                    ((ConfigurationObjectBase)clone).Clone((IConfigurationObject)value);
                    return (TElement)clone;
            }

            return value;
        }

        /// <summary>
        ///     Determines whether the current value is the same as the original value.
        /// </summary>
        /// <param name="original"> The original value. </param>
        /// <param name="current"> The current value. </param>
        /// <returns> <see langword="true" /> if the values are the same; otherwise, <see langword="false" />. </returns>
        internal static bool AreEqual(object? original, object? current)
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

            return UniversalComparer.Equals(original, current);
        }
    }
}