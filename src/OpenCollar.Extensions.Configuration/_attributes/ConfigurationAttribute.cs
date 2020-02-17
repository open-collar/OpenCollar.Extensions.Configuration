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

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     An attribute that can be used to specify the default value to return if no value is defined in the
    ///     configuration root.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ConfigurationAttribute : Attribute
    {
        /// <summary>
        ///     The default value to return if no value is defined in the configuration root. Can be <see langword="null" />.
        /// </summary>
        private object? _defaultValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationAttribute" /> class.
        /// </summary>
        public ConfigurationAttribute()
        {
            Persistence = ConfigurationPersistenceActions.LoadAndSave;
        }

        /// <summary>
        ///     Gets the default value.
        /// </summary>
        /// <value> The default value to return if no value is defined in the configuration root. Can be <see langword="null" />. </value>
        public object? DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                IsDefaultValueSet = true;
            }
        }

        /// <summary>
        ///     Gets or sets the policy for when to load and save values from the configuration service.
        /// </summary>
        /// <value> The policy for when to load and save values from the configuration service. </value>
        public ConfigurationPersistenceActions Persistence
        {
            get; set;
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="DefaultValue" /> property has been set.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the <see cref="DefaultValue" /> property has been set; otherwise, <see langword="false" />.
        /// </value>
        internal bool IsDefaultValueSet
        {
            get; private set;
        }
    }
}