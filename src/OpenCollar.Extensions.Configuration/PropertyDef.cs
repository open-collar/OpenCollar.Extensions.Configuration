using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// The definition of a property of a configuration object.
    /// </summary>
    internal class PropertyDef
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDef" /> class.
        /// </summary>
        /// <param name="path">The colon-delimited path to the underlying configuration value.</param>
        /// <param name="propertyName">The name of the property represented by this object.</param>
        /// <param name="type">The type of the value held in the property.</param>
        internal PropertyDef(string path, string propertyName, Type type)
        {
            Path = path;
            PropertyName = propertyName;
            Type = type;
        }

        /// <summary>
        /// Gets the colon-delimited path to the underlying configuration value.
        /// </summary>
        /// <value>The colon-delimited path to the underlying configuration value.</value>
        public string Path
        {
            get;
        }

        /// <summary>Gets the name of the property represented by this object.</summary>
        /// <value>The name of the property represented by this object.</value>
        public string PropertyName
        {
            get;
        }

        /// <summary>Gets the type of the value held in the property.</summary>
        /// <value>The type of the value held in the property.</value>
        public Type Type
        {
            get;
        }


    }
}
