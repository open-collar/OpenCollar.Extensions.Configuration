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

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>Defines the context in which the configuration object is being constructed.</summary>
    internal sealed class ConfigurationContext
    {
        /// <summary>The string used to delimit the sections of the path.</summary>
        public const string PathDelimiter = ":";

        /// <summary>Initializes a new instance of the <see cref="ConfigurationContext"/> class.</summary>
        public ConfigurationContext()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ConfigurationContext"/> class.</summary>
        /// <param name="context">The existing context from which to initialize.</param>
        public ConfigurationContext(ConfigurationContext context)
        {
            Path = context.Path;
        }

        /// <summary>Gets or sets the path.</summary>
        /// <value>The path.</value>
        public string Path { get; private set; } = string.Empty;

        /// <summary>Applies the path attribute to modify the current context.</summary>
        /// <param name="attribute">The path attribute to apply.</param>
        public void ApplyPathAttribute(PathAttribute attribute)
        {
            switch(attribute.Usage)
            {
                case PathIs.Root:
                    Path = attribute.Path;
                    break;

                case PathIs.Suffix:
                    if(string.IsNullOrWhiteSpace(Path))
                    {
                        Path = attribute.Path;
                    }
                    else
                    {
                        Path = string.Concat(Path, PathDelimiter, attribute.Path);
                    }

                    break;
            }
        }
    }
}