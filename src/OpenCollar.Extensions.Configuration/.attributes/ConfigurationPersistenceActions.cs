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
    ///     Defines the behavior when loading or saving the value for any particular property.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/.attributes/ConfigurationPersistenceActions/ConfigurationPersistenceActions.svg" />
    /// </remarks>
    /// <seealso cref="ConfigurationAttribute" />
    [Flags]
    public enum ConfigurationPersistenceActions
    {
        /// <summary>
        ///     Values will be neither loaded nor saved.
        /// </summary>
        /// <remarks>
        ///     To use this value a default value must be provided.
        /// </remarks>
        Ignore = 0,

        /// <summary>
        ///     Values are loaded from the configuration service, but changes are never saved back to the configuration service.
        /// </summary>
        LoadOnly = 1,

        /// <summary>
        ///     Values are never loaded from the configuration service, but changes are saved back to the configuration service.
        /// </summary>
        /// <remarks>
        ///     To use this value a default value must be provided.
        /// </remarks>
        SaveOnly = 2,

        /// <summary>
        ///     Values are loaded from the configuration service and changes are saved back to the configuration service.
        /// </summary>
        /// <remarks>
        ///     This is the default behavior.
        /// </remarks>
        LoadAndSave = LoadOnly | SaveOnly,
    }
}