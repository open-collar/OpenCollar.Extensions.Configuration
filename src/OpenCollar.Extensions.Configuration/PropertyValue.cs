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
                return _propertyDef.ConvertValueToString(_currentValue);
            }
            set
            {
                Value = _propertyDef.ConvertStringToValue(value);
            }
        }

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read the value. </param>
        /// <returns> </returns>
        public void DeleteValue(IConfigurationRoot configurationRoot)
        {
            configurationRoot[_propertyDef.Path] = null;
        }

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read the value. </param>
        /// <returns> </returns>
        public void ReadValue(IConfigurationRoot configurationRoot)
        {
            lock(_lock)
            {
                // If the value is a configuration object of some sort then create or reuse the existing value;
                IConfigurationObject? configurationObject;
                switch(_propertyDef.ImplementationKind)
                {
                    case ImplementationKind.ConfigurationCollection:
                    case ImplementationKind.ConfigurationDictionary:
                        configurationObject = (_currentValue ?? _originalValue) as IConfigurationObject;
                        if(ReferenceEquals(configurationObject, null) || (configurationObject.GetType() != _propertyDef.ImplementationType))
                        {
                            Value = Activator.CreateInstance(_propertyDef.ImplementationType, _propertyDef, configurationRoot);
                        }
                        else
                        {
                            configurationObject.Reload();
                            Value = configurationObject;
                        }
                        break;

                    case ImplementationKind.ConfigurationObject:
                        configurationObject = (_currentValue ?? _originalValue) as IConfigurationObject;
                        if(ReferenceEquals(configurationObject, null) || (configurationObject.GetType() != _propertyDef.ImplementationType))
                        {
                            Value = Activator.CreateInstance(_propertyDef.ImplementationType, configurationRoot);
                        }
                        else
                        {
                            configurationObject.Reload();
                            Value = configurationObject;
                        }
                        break;

                    default:
                        var value = configurationRoot[_propertyDef.Path];

                        if(ReferenceEquals(value, null) && !_propertyDef.IsNullable)
                        {
                            throw new ConfigurationException(_propertyDef.Path, $"No value could be found for configuration path: '{_propertyDef.Path}'.");
                        }

                        if(ReferenceEquals(value, null) && _propertyDef.IsNullable)
                        {
                            Value = _propertyDef.DefaultValue;
                        }
                        else
                        {
                            StringValue = value;
                        }
                        break;
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
                switch(_propertyDef.ImplementationKind)
                {
                    case ImplementationKind.ConfigurationCollection:
                    case ImplementationKind.ConfigurationDictionary:
                    case ImplementationKind.ConfigurationObject:
                        if(!ReferenceEquals(_originalValue, null) && ReferenceEquals(_currentValue, null))
                        {
                            // The value (and all it's sub nodes) has been deleted.
                            ((IConfigurationObject)_originalValue).Delete();
                        }
                        else
                        {
                            if(!ReferenceEquals(_currentValue, null))
                            {
                                ((IConfigurationObject)_currentValue).Save();
                            }
                            else
                            {
                                // TODO: Make sure all mention of the value has been deleted.
                            }
                        }
                        break;

                    default:
                        configurationRoot[_propertyDef.Path] = StringValue;
                        break;
                }
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

            var configurationObject = current as IConfigurationObject;
            if(!ReferenceEquals(configurationObject, null) && configurationObject.IsDirty)
            {
                return true;
            }

            return original.Equals(current);
        }
    }
}