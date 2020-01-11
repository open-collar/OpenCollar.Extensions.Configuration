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
 * Copyright © 2019 Jonathan Evans (jevans@open-collar.org.uk).
 */

using System.Collections.Generic;

using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A read-only dictionary of configuration objects, keyed on the name of the object.
    /// </summary>
    /// <typeparam name="TElement"> The type of the element. </typeparam>
    /// <seealso cref="ConfigurationDictionaryBase{T,T}" />
    /// <seealso cref="IConfigurationDictionary{TElement}" />
    internal sealed class ReadOnlyConfigurationDictionary<TElement> : ConfigurationDictionary<TElement>, IReadOnlyDictionary<string, TElement>
        where TElement : IConfigurationObject
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyConfigurationDictionary{TElement}" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        /// <param name="elements"> The elements with which to initialize to the collection. </param>
        public ReadOnlyConfigurationDictionary(PropertyDef propertyDef, IEnumerable<TElement>? elements) : base(propertyDef, elements)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyConfigurationDictionary{TElement}" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        /// <param name="elements">
        ///     A parameter array containing the elements with which to initialize to the collection.
        /// </param>
        public ReadOnlyConfigurationDictionary(PropertyDef propertyDef, params TElement[]? elements) : base(propertyDef, elements)
        {
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="ICollection{T}" /> is read-only.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets an <see cref="ICollection{TElement}" /> containing the keys of the <see cref="IDictionary{T,T}" />.
        /// </summary>
        IEnumerable<string> IReadOnlyDictionary<string, TElement>.Keys
        {
            get
            {
                return Keys;
            }
        }

        /// <summary>
        ///     Gets an <see cref="ICollection{TElement}" /> containing the values in the <see cref="IDictionary{T,T}" />.
        /// </summary>
        IEnumerable<TElement> IReadOnlyDictionary<string, TElement>.Values
        {
            get
            {
                return Values;
            }
        }
    }
}