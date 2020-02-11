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

using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     The interface internally exposed by value objects.
    /// </summary>
    public interface IValue
    {
        /// <summary>
        ///     Gets a value indicating whether this property has unsaved changes.
        /// </summary>
        /// <value> <see langword="true" /> if this property has unsaved changes; otherwise, <see langword="false" />. </value>
        bool IsDirty
        {
            get;
        }

        /// <summary>
        ///     Gets the parent object to which this value belongs.
        /// </summary>
        /// <value> The parent object to which this value belongs. </value>
        IValueChanged Parent
        {
            get;
        }

        /// <summary>
        ///     Gets the colon-delimited path to the underlying configuration value.
        /// </summary>
        /// <value> The colon-delimited path to the underlying configuration value. </value>
        string Path
        {
            get;
        }

        /// <summary>
        ///     Gets the definition of the property represented by this value.
        /// </summary>
        /// <value> The definition of the property represented by this value. </value>
        PropertyDef PropertyDef
        {
            get;
        }

        /// <summary>
        ///     Gets or sets the value of the property represented by this instance.
        /// </summary>
        /// <value> The value of the property. </value>
        object? Value
        {
            get; set;
        }

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read the value. </param>
        void DeleteValue(IConfigurationRoot configurationRoot);

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read the value. </param>
        void ReadValue(IConfigurationRoot configurationRoot);

        /// <summary>
        ///     Sets the value without firing any events.
        /// </summary>
        /// <param name="value"> The new value. </param>
        /// <returns> <see langword="true" /> if the value has changed; otherwise, <see langword="false" />. </returns>
        bool SetValue(object? value);

        /// <summary>
        ///     Writes the value to the configuration store.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root to which to write the value. </param>
        void WriteValue(IConfigurationRoot configurationRoot);
    }
}