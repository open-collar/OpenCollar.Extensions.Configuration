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
    ///     Defines a dictionary containing configuration items and keyed on the element name.
    /// </summary>
    /// <typeparam name="TElement">
    ///     The type of the elements contained in the dictionary.
    /// </typeparam>
    /// <seealso cref="IDictionary{TKey,TValue}" />
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/interfaces/IConfigurationDictionary/IConfigurationDictionary.svg" />
    /// </remarks>
    public interface IConfigurationDictionary<TElement> : IDictionary<string, TElement>, INotifyCollectionChanged
    {
        /// <summary>
        ///     Adds a new value with the key specified, copying the properties and elements from the value give,
        ///     returning the new value.
        /// </summary>
        /// <param name="key">
        ///     The key identifying the value to add.
        /// </param>
        /// <param name="value">
        ///     The value to copy.
        /// </param>
        /// <returns>
        ///     The newly added element.
        /// </returns>
        /// <remarks>
        ///     Used to add objects and collections that have been constructed externally using alternate implementations.
        /// </remarks>
        TElement AddCopy(string key, TElement value);

        /// <summary>
        ///     Adds a new value with the key specified, returning the new value.
        /// </summary>
        /// <param name="key">
        ///     The key identifying the value to add.
        /// </param>
        /// <returns>
        ///     The newly added element.
        /// </returns>
        TElement AddNew(string key);

        /// <summary>
        ///     Determines whether this dictionary contains the element specified.
        /// </summary>
        /// <param name="element">
        ///     The element for which to check.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the dictionary contains the specified element; otherwise, <see langword="false" />.
        /// </returns>
        bool Contains(TElement element);
    }
}