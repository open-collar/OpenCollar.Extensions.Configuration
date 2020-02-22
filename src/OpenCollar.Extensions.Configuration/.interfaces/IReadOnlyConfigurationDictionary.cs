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

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Defines a read-only dictionary containing configuration items and keyed on the element name.
    /// </summary>
    /// <typeparam name="TElement">
    ///     The type of the elements contained in the dictionary.
    /// </typeparam>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/.interfaces/IReadOnlyConfigurationDictionary/IReadOnlyConfigurationDictionary.svg" />
    /// </remarks>
    /// <seealso cref="System.Collections.Generic.IReadOnlyDictionary{TKey, TElement}" />
    /// <seealso cref="OpenCollar.Extensions.Configuration.IConfigurationDictionary{TElement}" />
    public interface IReadOnlyConfigurationDictionary<TElement> : IConfigurationDictionary<TElement>, IReadOnlyDictionary<string, TElement>
    {
    }
}