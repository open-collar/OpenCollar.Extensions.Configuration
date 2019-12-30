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
    internal class PropertyValue
    {
        /// <summary>
        ///     The parent configuration object for which this object represents a property.
        /// </summary>
        private readonly ConfigurationObjectBase _parent;

        /// <summary>
        ///     The type of the value held in the property.
        /// </summary>
        private readonly Type _type;

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
        /// <param name="value"> The current value held in this property. </param>
        internal PropertyValue(PropertyDef propertyDef, ConfigurationObjectBase parent, object? value)
        {
            _parent = parent;
            Path = propertyDef.Path;
            PropertyName = propertyDef.PropertyName;
            _type = propertyDef.Type;
            _originalValue = value;
            _currentValue = value;
        }

        /// <summary>
        ///     Gets a value indicating whether this property has unsaved changes.
        /// </summary>
        /// <value> <see langword="true" /> if this property has unsaved changes; otherwise, <see langword="false" />. </value>
        public bool IsDirty => !AreEqual(_originalValue, _currentValue);

        /// <summary>
        ///     Gets the colon-delimited path to the underlying configuration value.
        /// </summary>
        /// <value> The colon-delimited path to the underlying configuration value. </value>
        public string Path { get; }

        /// <summary>
        ///     Gets the name of the property represented by this object.
        /// </summary>
        /// <value> The name of the property represented by this object. </value>
        public string PropertyName { get; }

        /// <summary>
        ///     Gets or sets the value of the property represented by this instance.
        /// </summary>
        /// <value> The value of the property. </value>
        public object? Value
        {
            get => _currentValue;
            set
            {
                if(AreEqual(_originalValue, value))
                {
                    return;
                }

                _currentValue = value;

                _parent.OnPropertyChanged(this);
            }
        }

        public void ReadValue(IConfigurationRoot configurationRoot)
        {
        }

        public void WriteValue(IConfigurationRoot configurationRoot)
        {
        }

        /// <summary>
        ///     Called when the current value has been saved to he underlying configuration source.
        /// </summary>
        internal void Saved()
        {
            _originalValue = _currentValue;
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
                return false;
            }

            if(ReferenceEquals(original, null) || ReferenceEquals(current, null))
            {
                return true;
            }

            return original.Equals(current);
        }
    }
}