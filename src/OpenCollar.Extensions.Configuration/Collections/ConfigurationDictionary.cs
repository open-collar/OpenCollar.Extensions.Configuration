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
    ///     A dictionary of configuration objects, keyed on the name of the object.
    /// </summary>
    /// <typeparam name="TElement"> The type of the element. </typeparam>
    internal class ConfigurationDictionary<TElement> : ConfigurationDictionaryBase<string, TElement>, IConfigurationDictionary<TElement>
            where TElement : IConfigurationObject
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationDictionary{TElement}" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        public ConfigurationDictionary(PropertyDef propertyDef) : base(propertyDef)
        {
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="System.Collections.Generic.ICollection{TElement}" /> is read-only.
        /// </summary>
        public override bool IsReadOnly { get { return false; } }

        /// <summary>
        ///     Gets a value indicating whether to set values using the key first.
        /// </summary>
        /// <value> <see langword="true" /> if set value using key first; otherwise to value first, <see langword="false" />. </value>
        protected override bool SetValueUsingKeyFirst { get { return true; } }

        /// <summary>
        ///     Adds the specified element to the dictionary, using the name of the object as a the key.
        /// </summary>
        /// <param name="element"> The element to add to the dictionary. </param>
        public void Add(TElement element)
        {
            Add(element.PropertyDef.PropertyName, element);
        }

        /// <summary>
        ///     Adds an element with the provided key and value to the <see cref="System.Collections.Generic.IDictionary{TKey, TElement}" />.
        /// </summary>
        /// <param name="key"> The object to use as the key of the element to add. </param>
        /// <param name="value"> The object to use as the value of the element to add. </param>
        /// <exception cref="System.ArgumentException">
        ///     The <paramref name="key" /> provided does not match the name of the element given.
        /// </exception>
        public override void Add(string key, TElement value)
        {
            if(!Equals(key, value.PropertyDef.PropertyName))
            {
                throw new System.ArgumentException($"The '{nameof(key)}' provided does not match the name of the element given.", nameof(value));
            }

            base.Add(key, value);
        }

        /// <summary>
        ///     Determines whether this instance contains the object.
        /// </summary>
        /// <param name="element"> The element for which to check. </param>
        /// <returns> <see langword="true" /> if the dictionary contains the specified element; otherwise, <see langword="false" />. </returns>
        public bool Contains(TElement element) => ContainsValue(element);

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns> An enumerator that can be used to iterate through the collection. </returns>
        public IEnumerator<KeyValuePair<string, TElement>> GetEnumerator()
        {
            EnforceDisposed();

            Lock.EnterReadLock();
            try
            {
                return OrderedItems.GetEnumerator();
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Determines whether this instance contains the object given.
        /// </summary>
        /// <param name="item"> The object to locate in the <see cref="System.Collections.Generic.ICollection{TElement}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> is found in the
        ///     <see cref="System.Collections.Generic.ICollection{TElement}" />; otherwise, <see langword="false" />.
        /// </returns>
        bool ICollection<KeyValuePair<string, TElement>>.Contains(KeyValuePair<string, TElement> item) => ContainsKey(item.Key);
    }
}