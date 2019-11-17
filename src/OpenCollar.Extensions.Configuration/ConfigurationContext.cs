using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// Defines the context in which the configuration object is being constructed.
    /// </summary>
    internal sealed class ConfigurationContext
    {
        /// <summary>The string used to delimit the sections of the path.</summary>
        public const string PathDelimiter = ":";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationContext"/> class.
        /// </summary>
        public ConfigurationContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationContext"/> class.
        /// </summary>
        /// <param name="context">The existing context from which to initialize.</param>
        public ConfigurationContext(ConfigurationContext context)
        {
            Path = context.Path;
        }

        /// <summary>Gets or sets the path.</summary>
        /// <value>The path.</value>
        public string Path
        {
            get; private set;
        } = string.Empty;

        /// <summary>Applies the path attribute to modify the current context.</summary>
        /// <param name="attribute">The path attribute to apply.</param>
        public void ApplyPathAttribute(PathAttribute attribute)
        {
            switch (attribute.Usage)
            {
                case PathIs.Root:
                    Path = attribute.Path;
                    break;

                case PathIs.Suffix:
                    if (string.IsNullOrWhiteSpace(Path))
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
