using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// Specifies the ways in which a property can be used to represent structural elements.
    /// </summary>
    public enum StructuralElementKind
    {
        /// <summary>The kind of structural element is unknown or undefined.  Use of this value will usually result in an
        /// error; it is provided to as sentinel to detect accidental usages.</summary>
        Unknown = 0,

        /// <summary>The element is a property that contains a single, unstructured, value.</summary>
        Property,

        /// <summary>The property contains an array of elements.</summary>
        Array,

        /// <summary>The property contains a dictionary of elements.</summary>
        Dictionary
    }

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
        /// <param name="isReadOnly">If set to <see langword="true" /> the property is read-only; otherwise, <see langword="false"/> indicates that the property is editable.</param>
        internal PropertyDef(string path, string propertyName, Type type, bool isReadOnly)
        {
            Path = path;
            PropertyName = propertyName;
            Type = type;
            IsReadOnly = isReadOnly;
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

        /// <summary>Gets a value indicating whether this instance is read only.</summary>
        /// <value>
        ///   <see langword="true" /> if the property is read only; otherwise, <see langword="false" /> for an editable property.
        /// </value>
        public bool IsReadOnly
        {
            get;
        }
    }
}
