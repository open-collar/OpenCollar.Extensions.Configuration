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

using System;

using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A class used to represent a property on an interface and its location in the configuration model.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Path,nq}=\"{StringValue}\"")]
    internal sealed class PropertyValue
    {
        /// <summary>
        ///     A lock token used to control concurrent access to the <see cref="_currentValue" /> and
        ///     <see cref="_originalValue" /> fields.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        ///     The parent configuration object for which this object represents a property.
        /// </summary>
        private readonly ConfigurationObjectBase _parent;

        /// <summary>
        ///     The definition of the property represented by this value.
        /// </summary>
        private readonly PropertyDef _propertyDef;

        /// <summary>
        ///     The current (unsaved) value held in this property.
        /// </summary>
        /// <remarks>
        ///     This will be used to compare against the <see cref="_originalValue" /> to determine whether the value
        ///     has changed/
        /// </remarks>
        private object? _currentValue;

        /// <summary>
        ///     The original (saved) value of the property.
        /// </summary>
        private object? _originalValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyValue" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property to represent. </param>
        /// <param name="parent"> The parent configuration object for which this object represents a property. </param>
        internal PropertyValue(PropertyDef propertyDef, ConfigurationObjectBase parent)
        {
            _parent = parent;
            _propertyDef = propertyDef;
        }

        /// <summary>
        ///     Gets a value indicating whether this property has unsaved changes.
        /// </summary>
        /// <value> <see langword="true" /> if this property has unsaved changes; otherwise, <see langword="false" />. </value>
        public bool IsDirty
        {
            get
            {
                lock(_lock)
                {
                    return !AreEqual(_originalValue, _currentValue);
                }
            }
        }

        /// <summary>
        ///     Gets the colon-delimited path to the underlying configuration value.
        /// </summary>
        /// <value> The colon-delimited path to the underlying configuration value. </value>
        public string Path
        {
            get
            {
                return _propertyDef.Path;
            }
        }

        /// <summary>
        ///     Gets the name of the property represented by this object.
        /// </summary>
        /// <value> The name of the property represented by this object. </value>
        public string PropertyName
        {
            get
            {
                return _propertyDef.PropertyName;
            }
        }

        /// <summary>
        ///     Gets or sets the value of the property represented by this instance.
        /// </summary>
        /// <value> The value of the property. </value>
        public object? Value
        {
            get => _currentValue;
            set
            {
                lock(_lock)
                {
                    if(AreEqual(_originalValue, value))
                    {
                        return;
                    }

                    _currentValue = value;
                }

                _parent.OnPropertyChanged(this);
            }
        }

        /// <summary>
        ///     Gets or sets the value of the property, represented as a string.
        /// </summary>
        /// <value> The value of the property, represented as a string. </value>
        internal string? StringValue
        {
            get
            {
                var value = _currentValue;
                if(ReferenceEquals(value, null))
                {
                    return null;
                }

                var type = _propertyDef.UnderlyingType;

                if(type.IsEnum)
                {
                    return Enum.GetName(type, value);
                }

                if(type == typeof(string))
                {
                    return (string)value;
                }

                if(type == typeof(System.Char))
                {
                    return new string(((System.Char)value), 1);
                }

                if(type == typeof(System.Int16))
                {
                    return ((System.Int16)value).ToString("D", System.Globalization.CultureInfo.InvariantCulture);
                }

                if(type == typeof(System.SByte))
                {
                    return ((System.SByte)value).ToString("D", System.Globalization.CultureInfo.InvariantCulture);
                }

                if(type == typeof(System.Int32))
                {
                    return ((System.Int32)value).ToString("D", System.Globalization.CultureInfo.InvariantCulture);
                }

                if(type == typeof(System.Int64))
                {
                    return ((System.Int64)value).ToString("D", System.Globalization.CultureInfo.InvariantCulture);
                }

                if(type == typeof(System.Single))
                {
                    return ((System.Single)value).ToString("G9", System.Globalization.CultureInfo.InvariantCulture);
                }

                if(type == typeof(System.Double))
                {
                    return ((System.Double)value).ToString("G17", System.Globalization.CultureInfo.InvariantCulture);
                }

                if(type == typeof(System.Decimal))
                {
                    return ((System.Decimal)value).ToString("G", System.Globalization.CultureInfo.InvariantCulture);
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

                if(type == typeof(System.Boolean))
                {
                    return ((System.Boolean)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                }

                // For anything else, let's hope that "ToString" is good enough.
                return value.ToString();
            }
            set
            {
                if(ReferenceEquals(value, null))
                {
                    if(_propertyDef.IsNullable)
                    {
                        Value = _propertyDef.DefaultValue;
                        return;
                    }
                    else
                    {
                        throw new ConfigurationException(_propertyDef.Path, $"Null value cannot be assigned to configuration path: '{_propertyDef.Path}'.");
                    }
                }

                var type = _propertyDef.UnderlyingType;

                var parsedValue = ParseValue(value, type);

                Value = parsedValue;
            }
        }

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read the value. </param>
        /// <returns> </returns>
        public void ReadValue(IConfigurationRoot configurationRoot)
        {
            var value = configurationRoot[_propertyDef.Path];

            if(ReferenceEquals(value, null) && !_propertyDef.IsNullable)
            {
                throw new ConfigurationException(_propertyDef.Path, $"No value could be found for configuration path: '{_propertyDef.Path}'.");
            }

            lock(_lock)
            {
                if(ReferenceEquals(value, null) && _propertyDef.IsNullable)
                {
                    Value = _propertyDef.DefaultValue;
                }
                else
                {
                    StringValue = value;
                }
                Saved();
            }
        }

        /// <summary>
        ///     Writes the value to the configuration store.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root to which to write the value. </param>
        public void WriteValue(IConfigurationRoot configurationRoot)
        {
            lock(_lock)
            {
                configurationRoot[_propertyDef.Path] = StringValue;
                Saved();
            }
        }

        /// <summary>
        ///     Called when the current value has been saved to he underlying configuration source.
        /// </summary>
        internal void Saved()
        {
            lock(_lock)
            {
                _originalValue = _currentValue;
            }
        }

        /// <summary>
        ///     Determines whether the current value is the same as the original value.
        /// </summary>
        /// <param name="original"> The original value. </param>
        /// <param name="current"> The current value. </param>
        /// <returns> <see langword="true" /> if the values are the same; otherwise, <see langword="false" />. </returns>
        private static bool AreEqual(object? original, object? current)
        {
            if(ReferenceEquals(original, current))
            {
                return true;
            }

            if(ReferenceEquals(original, null) || ReferenceEquals(current, null))
            {
                return false;
            }

            return original.Equals(current);
        }

        /// <summary>
        ///     Parses a string value into the type defined by the property definition.
        /// </summary>
        /// <param name="value"> The string to parse. </param>
        /// <param name="type"> The type of the property. </param>
        /// <returns> The string parsed as the type of this property. </returns>
        /// <exception cref="ConfigurationException"> Value could not be converted. </exception>
        private object? ParseValue(string value, Type type)
        {
            if(type.IsEnum)
            {
                return Enum.Parse(type, value);
            }

            if(type == typeof(string))
            {
                return value;
            }

            if(type == typeof(System.Char))
            {
                if(value.Length != 1)
                {
                    throw new ConfigurationException(_propertyDef.Path, $"Value could not be treated as a 'char'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
                }
                return value[0];
            }

            if(type == typeof(System.Int16))
            {
                if(System.Int16.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(_propertyDef.Path, $"Value could not be parsed as an 'Int16'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
            }

            if(type == typeof(System.SByte))
            {
                if(System.SByte.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(_propertyDef.Path, $"Value could not be parsed as an 'SByte'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
            }

            if(type == typeof(System.Int32))
            {
                if(System.Int32.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var int32Value))
                {
                    return int32Value;
                }
                throw new ConfigurationException(_propertyDef.Path, $"Value could not be parsed as an 'Int32'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
            }

            if(type == typeof(System.Int64))
            {
                if(System.Int64.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var int64Value))
                {
                    return int64Value;
                }
                throw new ConfigurationException(_propertyDef.Path, $"Value could not be parsed as an 'Int64'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
            }

            if(type == typeof(System.Single))
            {
                if(System.Single.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(_propertyDef.Path, $"Value could not be parsed as an 'Single'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
            }

            if(type == typeof(System.Double))
            {
                if(System.Double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(_propertyDef.Path, $"Value could not be parsed as an 'Double'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
            }

            if(type == typeof(System.Decimal))
            {
                if(System.Decimal.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(_propertyDef.Path, $"Value could not be parsed as an 'Decimal'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
            }

            if(type == typeof(DateTime))
            {
                if(System.DateTime.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(_propertyDef.Path, $"Value could not be parsed as an 'DateTime'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
            }

            if(type == typeof(DateTimeOffset))
            {
                if(System.DateTimeOffset.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(_propertyDef.Path, $"Value could not be parsed as an 'DateTimeOffset'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
            }

            if(type == typeof(TimeSpan))
            {
                if(System.TimeSpan.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(_propertyDef.Path, $"Value could not be parsed as an 'TimeSpan'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
            }

            if(type == typeof(System.Boolean))
            {
                if(System.Boolean.TryParse(value, out var parsed))
                {
                    return parsed;
                }
                throw new ConfigurationException(_propertyDef.Path, $"Value could not be parsed as an 'Boolean'; configuration path: '{_propertyDef.Path}'; value: '{value}'.");
            }

            return Convert.ChangeType(value, _propertyDef.Type);
        }
    }
}