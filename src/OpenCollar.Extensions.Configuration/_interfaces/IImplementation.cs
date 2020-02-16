using System;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     The details of the implementation of a property or element.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IImplementation
    {
        /// <summary>
        ///     Gets the kind of the implementation to use to instantiate values.
        /// </summary>
        /// <value> The kind of the implementation to use to instantiate values. </value>
        ImplementationKind ImplementationKind
        {
            get;
        }

        /// <summary>
        ///     Gets the type of the object that implements values ( <see langword="null" /> if the property is naive).
        /// </summary>
        /// <value>
        ///     The type of the object that implements values ( <see langword="null" /> if the property is naive).
        /// </value>
        Type? ImplementationType
        {
            get;
        }

        /// <summary>
        ///     Gets the type of the value represented (the type of the property).
        /// </summary>
        /// <value> The type of the value represented (the type of the property). </value>
        Type Type
        {
            get;
        }
    }
}