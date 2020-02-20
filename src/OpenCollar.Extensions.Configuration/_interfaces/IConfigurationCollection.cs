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
    ///     Represents a collection of values stored in a property.
    /// </summary>
    /// <typeparam name="TElement">
    ///     The type of the array element. This must be nullable if the type is a reference type and can be <see langword="null" />.
    /// </typeparam>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/_interfaces/IConfigurationCollection/IConfigurationCollection.svg" />
    /// </remarks>
    public interface IConfigurationCollection<TElement> : IList<TElement>, INotifyCollectionChanged
    {
        /// <summary>
        ///     Adds a new value with the key specified, copying the properties and elements from the value give,
        ///     returning the new value.
        /// </summary>
        /// <param name="value"> The value to copy. </param>
        /// <returns> The newly added element. </returns>
        /// <remarks>
        ///     Used to add objects and collections that have been constructed externally using alternate implementations.
        /// </remarks>
        TElement AddCopy(TElement value);

        /// <summary>
        ///     Adds a new value with the key specified, returning the new value.
        /// </summary>
        /// <returns> The newly added element. </returns>
        TElement AddNew();
    }
}