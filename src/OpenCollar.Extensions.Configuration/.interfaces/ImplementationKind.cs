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

using System.ComponentModel;

using JetBrains.Annotations;

using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Defines the way in which the value returned by a property is implemented.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/.interfaces/ImplementationKind/ImplementationKind.svg" />
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum ImplementationKind
    {
        /// <summary>
        ///     The implementation is unknown or undefined.
        /// </summary>
        [UsedImplicitly]
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
        ///     The implementation is derived from <see cref="ConfigurationCollection{TElement}" />.
        /// </summary>
        ConfigurationCollection,

        /// <summary>
        ///     The implementation is derived from <see cref="ConfigurationDictionary{TInterface}" />.
        /// </summary>
        ConfigurationDictionary
    }
}