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
using System.ComponentModel;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     The details of the implementation of a property or element.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IImplementation
    {
        /// <summary>
        ///     Gets the kind of the implementation to use to instantiate values.
        /// </summary>
        /// <value> The kind of the implementation to use to instantiate values. </value>
        ImplementationKind ImplementationKind
        {
            get;
        }

        /// <summary>
        ///     Gets the type of the object that implements values ( <see langword="null" /> if the property is naive).
        /// </summary>
        /// <value>
        ///     The type of the object that implements values ( <see langword="null" /> if the property is naive).
        /// </value>
        Type? ImplementationType
        {
            get;
        }

        /// <summary>
        ///     Gets the type of the value represented (the type of the property).
        /// </summary>
        /// <value> The type of the value represented (the type of the property). </value>
        Type Type
        {
            get;
        }
    }
}