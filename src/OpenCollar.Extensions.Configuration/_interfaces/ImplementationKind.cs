using System.ComponentModel;

using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary> Defines the way in which the value returned by a property is implemented. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum ImplementationKind
    {
        /// <summary> The implementation is unknown or undefined. </summary>
        Unknown = 0,

        /// <summary> The implementation is the naive type (i.e. nothing special is required). </summary>
        Naive,

        /// <summary> The implementation is derived from <see cref="ConfigurationObjectBase{TInterface}"/>. </summary>
        ConfigurationObject,

        /// <summary> The implementation is derived from <see cref="ConfigurationCollection{TElement}"/>. </summary>
        ConfigurationCollection,

        /// <summary> The implementation is derived from <see cref="ConfigurationDictionary{TInterface}"/>. </summary>
        ConfigurationDictionary
    }
}