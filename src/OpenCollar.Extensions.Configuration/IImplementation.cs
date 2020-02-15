using System;
using System.Diagnostics;
using System.Linq;

using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Defines the way in which the value returned by a property is implemented.
    /// </summary>
    public enum ImplementationKind
    {
        /// <summary>
        ///     The implementation is unknown or undefined.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     The implementation is the naive type (i.e. nothing special is required).
        /// </summary>
        Naive,

        /// <summary>
        ///     The implementation is derived from <see cref="ConfigurationObjectBase{TInterface}" />.
        /// </summary>
        ConfigurationObject,

        /// <summary>
        ///     The implementation is derived from <see cref="ConfigurationCollection{TElement}" />.
        /// </summary>
        ConfigurationCollection,

        /// <summary>
        ///     The implementation is derived from <see cref="ConfigurationDictionary{TInterface}" />.
        /// </summary>
        ConfigurationDictionary
    }

    /// <summary>
    ///     The details of the implementation of a property or element.
    /// </summary>
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