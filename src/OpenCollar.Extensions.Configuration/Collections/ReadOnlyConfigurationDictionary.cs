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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration.Collections
{
    /// <summary>
    ///     A read-only dictionary of configuration objects, keyed on the name of the object.
    /// </summary>
    /// <typeparam name="TElement"> The type of the element. </typeparam>
    /// <seealso cref="ConfigurationDictionaryBase{TKey,TElement}" />
    /// <seealso cref="IConfigurationDictionary{TElement}" />
    [DebuggerDisplay("\\{ReadOnlyConfigurationDictionary<{typeof(TElement).Name,nq}>\\}: \"{" + nameof(CalculatePath) + "(),nq}\"")]
    internal sealed class ReadOnlyConfigurationDictionary<TElement> : ConfigurationDictionary<TElement>, IReadOnlyConfigurationDictionary<TElement>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationDictionary{TElement}" /> class.
        /// </summary>
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        /// <param name="configurationRoot">
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </param>
        /// <param name="elements"> The elements with which to initialize to the collection. </param>
        public ReadOnlyConfigurationDictionary(IConfigurationParent? parent, IPropertyDef propertyDef, IConfigurationRoot configurationRoot,
        IEnumerable<KeyValuePair<string, TElement>>? elements) : base(parent, propertyDef, configurationRoot, elements)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationDictionary{TElement}" /> class.
        /// </summary>
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        /// <param name="configurationRoot">
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </param>
        /// <param name="elements">
        ///     A parameter array containing the elements with which to initialize to the collection.
        /// </param>
        public ReadOnlyConfigurationDictionary(IConfigurationParent? parent, IPropertyDef propertyDef, IConfigurationRoot configurationRoot,
        params KeyValuePair<string, TElement>[]? elements) : base(parent, propertyDef, configurationRoot, elements)
        {
        }

        /// <summary>
        ///     Gets an <see cref="ICollection{TElement}" /> containing the keys of the <see cref="IDictionary{T,T}" />.
        /// </summary>
        IEnumerable<string> IReadOnlyDictionary<string, TElement>.Keys => Keys;

        /// <summary>
        ///     Gets an <see cref="ICollection{TElement}" /> containing the values in the <see cref="IDictionary{T,T}" />.
        /// </summary>
        IEnumerable<TElement> IReadOnlyDictionary<string, TElement>.Values => Values;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="ICollection{TElement}" /> is read-only.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override bool InnerIsReadOnly => true;

        /// <summary>
        ///     Gets the element with the specified key.
        /// </summary>
        /// <value> The element requested. </value>
        /// <param name="key"> The key identify the element. </param>
        /// <returns> The value of the element specified </returns>
        TElement IReadOnlyDictionary<string, TElement>.this[string key] => base[key].Value;

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns> An <see cref="IEnumerator" /> object that can be used to iterate through the collection. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            EnforceDisposed();

            return ((IEnumerable)OrderedItems.Select(e => new KeyValuePair<string, TElement>(e.Key, e.Value))).GetEnumerator();
        }

        /// <summary>
        ///     Converts the string given to the key.
        /// </summary>
        /// <param name="key"> The key to convert, as a string. </param>
        /// <returns> Returns the key converted to the correct type. </returns>
        internal override string ConvertStringToKey(string key)
        {
            return key;
        }
    }
}