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

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     An attribute that can be used to specify the default value to return if no value is defined in the
    ///     configuration root.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DefaultValueAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultValueAttribute" /> class.
        /// </summary>
        /// <param name="defaultValue">
        ///     The default value to return if no value is defined in the configuration root. Can be <see langword="null" />.
        /// </param>
        public DefaultValueAttribute(object? defaultValue)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        ///     Gets the default value.
        /// </summary>
        /// <value> The default value to return if no value is defined in the configuration root. Can be <see langword="null" />. </value>
        public object? DefaultValue
        {
            get;
        }
    }
}