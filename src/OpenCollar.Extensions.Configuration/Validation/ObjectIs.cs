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

namespace OpenCollar.Extensions.Configuration.Validation
{
#pragma warning disable S2342 // Rename this enumeration to match the regular expression: '^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$'.

    /// <summary>
    ///     Defines the type of validation to apply to an <see cref="object" /> argument.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/Validation/ObjectIs/ObjectIs.svg" />
    /// </remarks>
    [Flags]
    internal enum ObjectIs
    {
        /// <summary>
        ///     The validation to perform is unknown or undefined. This value must never be used.
        /// </summary>
        None = 0,

        /// <summary>
        ///     The argument being validated must not be <see langword="null" />.
        /// </summary>
        NotNull = 1
    }
}