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
using System.ComponentModel;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     The interface from which all configuration objects are derived.
    /// </summary>
    public interface IConfigurationObject : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        ///     Gets a value indicating whether this object has any properties with unsaved changes.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if this object has any properties with unsaved changes; otherwise,
        ///     <see langword="false" /> .
        /// </value>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        bool IsDirty
        {
            get;
        }

        /// <summary>
        ///     Gets the definition of this property object.
        /// </summary>
        /// <value> The definition of this property object. </value>
        PropertyDef? PropertyDef
        {
            get;
        }

        /// <summary>
        ///     Recursively deletes all of the properties from the configuration sources.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        void Delete();

        /// <summary>
        ///     Loads all of the properties from the configuration sources, overwriting any unsaved changes.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        void Reload();

        /// <summary>
        ///     Saves this current values for each property back to the configuration sources.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        void Save();
    }
}