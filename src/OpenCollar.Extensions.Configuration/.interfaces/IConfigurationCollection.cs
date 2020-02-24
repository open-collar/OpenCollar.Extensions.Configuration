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

using System.Collections.Generic;
using System.Collections.Specialized;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Represents a collection of values stored in a property.  Use the interface for properties
    ///     representing an arbitrary number of the same kind of object identified by index.
    /// </summary>
    /// <typeparam name="TElement">
    ///     The type of the collection element. This must be nullable if the type is a reference type and can be <see langword="null" />.
    /// </typeparam>
    /// <example>
    ///     In the example below a property on an interface is defined as containing an arbitray
    ///     number of strings.
    ///         <code lang="c#">
    ///IConfigurationCollection&lt;string&gt; Names
    ///{
    ///    get; set;
    ///}
    ///     </code>
    ///     If the configuration is specified in a JSON configuration file it would look
    ///     something like this:
    ///     <code lang="json">
    ///{
    ///    "Names": [
    ///         "Value 1",
    ///         "Value 2",
    ///         "Value 3"
    ///    ]
    ///}
    ///     </code>
    /// </example>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/.interfaces/IConfigurationCollection/IConfigurationCollection.svg" />
    /// </remarks>
    /// <seealso cref="IList{TValue}" />
    /// <seealso cref="INotifyCollectionChanged" />
    /// <seealso cref="IConfigurationObject" />
    public interface IConfigurationCollection<TElement> : IList<TElement>, INotifyCollectionChanged, IConfigurationObject
    {
        /// <summary>
        ///     Adds a new value with the key specified, copying the properties and elements from the value give,
        ///     returning the new value.
        /// </summary>
        /// <param name="value">
        ///     The value to copy (must be compatible with the type or interface specified by <typeparamref name="TElement" />).
        /// </param>
        /// <returns>
        ///     The newly added element. If this element is derived from <see cref="IConfigurationObject" />,
        ///     <see cref="IConfigurationCollection{TElement}" /> or <see cref="IConfigurationDictionary{TElement}" />,
        ///     and the object given was not created by this library, then a new object will be created and they
        ///     properties and elements in the object given will be copied.
        /// </returns>
        /// <remarks>
        ///     Used to add objects and collections that have been constructed externally using alternate implementations.
        /// </remarks>
        TElement AddCopy(TElement value);

        /// <summary>
        ///     Adds a new value with the key specified, returning the newly created object.
        /// </summary>
        /// <returns>
        ///     The newly added element.
        /// </returns>
        /// <remarks>
        ///     This allows instances of the internal implementation of objects to be created and added to the
        ///     collection, and returned to be populated.
        /// </remarks>
        TElement AddNew();
    }
}