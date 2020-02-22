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

namespace OpenCollar.Extensions.Configuration
{
#pragma warning disable CA1717 // Just because it ends in an S doesn't mean it is a plural!

    /// <summary>
    ///     An enumeration of the ways in which the string supplied to the <see cref="PathAttribute"> Path </see>
    ///     attribute can be used to create a full path.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/attributes/PathIs/PathIs.svg" />
    /// </remarks>
    public enum PathIs
    {
        /// <summary>
        ///     The usage of the path is unknown or undefined. Use of this value will usually result in an error; it is
        ///     provided to as sentinel to detect accidental usages.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     The path is treated as a root and any previous context is ignored.
        /// </summary>
        Root,

        /// <summary>
        ///     The path is treated as a suffix to be applied (as part of colon delimited list) to the existing path context.
        /// </summary>
        Suffix
    }
}