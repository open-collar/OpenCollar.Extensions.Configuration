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
using System.Globalization;

using OpenCollar.Extensions.Configuration.Resources;

namespace OpenCollar.Extensions.Configuration
{
#pragma warning restore CA1717

    /// <summary>
    ///     Defines an attribute used to indicate the path to the configuration value(s) underlying a class or
    ///     individual property.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <c> usage </c> and <c> path </c> arguments of the constructor can be used to determine whether this
    ///         property is read from a path relative to the parent or from an absolute path.
    ///     </para>
    ///     <para>
    ///         The following UML has been generated directly from the source code using
    ///         <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/.attributes/PathAttribute/PathAttribute.svg" />
    ///     </para>
    /// </remarks>
    /// <seealso cref="Attribute" />
    /// <seealso cref="PathIs" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class PathAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PathAttribute" /> class.
        /// </summary>
        /// <param name="usage">
        ///     The usage.
        /// </param>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="path" /> must contain a valid path or fragment of a path.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="usage" /> does not contain a valid value.
        /// </exception>
        public PathAttribute(PathIs usage, string path)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_Path, nameof(path)), nameof(path));
            }

            switch(usage)
            {
                case PathIs.Root:
                case PathIs.Suffix:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(usage), usage,
                    string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_EnumDoesNotContainValidValue, nameof(usage)));
            }

            Usage = usage;
            Path = path;
        }

        /// <summary>
        ///     Gets the full path or fragment of a path specified.
        /// </summary>
        /// <value>
        ///     The full path or fragment of a path specified.
        /// </value>
        internal string Path
        {
            get;
        }

        /// <summary>
        ///     Gets the usage of the path or fragment of a path specified.
        /// </summary>
        /// <value>
        ///     The usage of the path or fragment of a path specified.
        /// </value>
        internal PathIs Usage
        {
            get;
        }
    }
}