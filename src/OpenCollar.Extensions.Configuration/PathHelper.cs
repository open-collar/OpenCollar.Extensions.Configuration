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

using System.Text;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Defines the context in which the configuration object is being constructed.
    /// </summary>
    internal static class PathHelper
    {
        /// <summary>
        ///     The string used to delimit the sections of the path.
        /// </summary>
        public const string PathDelimiter = ":";

        /// <summary>
        ///     Returns the sections of a path concatentated into a fully delimited path.
        /// </summary>
        /// <param name="sections">
        ///     The sections of the path to generate. <see langword="null" /> or zero-length sections are ignored.
        /// </param>
        /// <returns>
        ///     A string containing the sections of path given, delimited using <see cref="PathDelimiter" />. Will be an
        ///     empty string if no valid sections are supplied.
        /// </returns>
        public static string GetPath(params string[] sections)
        {
            if(ReferenceEquals(sections, null))
            {
                return string.Empty;
            }

            if(sections.Length <= 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            foreach(var section in sections)
            {
                if(string.IsNullOrWhiteSpace(section))
                {
                    continue;
                }

                if(builder.Length > 0)
                {
                    builder.Append(PathDelimiter);
                }

                builder.Append(section);
            }

            return builder.ToString();
        }
    }
}