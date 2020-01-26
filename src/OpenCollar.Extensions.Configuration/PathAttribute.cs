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
    ///     An enumeration of the ways in which the string supplied to the <see cref="PathAttribute"> Path </see>
    ///     attribute can be used to create a full path.
    /// </summary>
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
        ///     The path is treated as a suffix to be applied (as part of colon delited list) to the existing path context.
        /// </summary>
        Suffix
    }

    /// <summary>
    ///     Defines an attribute used to indicate the path to the configuration value(s) underlying a class or
    ///     individual property.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class PathAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PathAttribute" /> class.
        /// </summary>
        /// <param name="usage"> The usage. </param>
        /// <param name="path"> The path. </param>
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
                throw new ArgumentException($"'{nameof(path)}' must contain a valid path or fragment of a path.", nameof(path));
            }

            switch(usage)
            {
                case PathIs.Root:
                case PathIs.Suffix:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(usage), usage, "'usage' does not contain a valid value.");
            }

            Usage = usage;
            Path = path;
        }

        /// <summary>
        ///     Gets the full path or fragment of a path specified.
        /// </summary>
        /// <value> The full path or fragment of a path specified. </value>
        internal string Path
        {
            get;
        }

        /// <summary>
        ///     Gets the usage of the path or fragment of a path specified.
        /// </summary>
        /// <value> The usage of the path or fragment of a path specified. </value>
        internal PathIs Usage
        {
            get;
        }
    }
}