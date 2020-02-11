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
    ///     A dictionary of configuration objects, keyed on the name of the object.
    /// </summary>
    /// <typeparam name="TElement"> The type of the element. </typeparam>
    /// <seealso cref="ConfigurationDictionaryBase{TKey,TElement}" />
    /// <seealso cref="IConfigurationDictionary{TElement}" />
    [DebuggerDisplay("ConfigurationDictionary[{Count}] ({GetPath()})")]
    public class ConfigurationDictionary<TElement> : ConfigurationDictionaryBase<string, TElement>, IConfigurationDictionary<TElement>
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
        public ConfigurationDictionary(IConfigurationParent? parent, PropertyDef propertyDef, IConfigurationRoot configurationRoot,
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
        public ConfigurationDictionary(IConfigurationParent? parent, PropertyDef propertyDef, IConfigurationRoot configurationRoot,
            params KeyValuePair<string, TElement>[]? elements) : base(parent, propertyDef, configurationRoot, elements)
        {
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="ICollection{TElement}" /> is read-only.
        /// </summary>
        protected override bool InnerIsReadOnly => false;

        /// <summary>
        ///     Gets or sets the element with the specified key.
        /// </summary>
        /// <value> The specified element. </value>
        /// <param name="key"> The key identifying the element required. </param>
        /// <returns> </returns>
        TElement IDictionary<string, TElement>.this[string key]
        {
            get => base[key].Value;
            set => base[key].Value = value;
        }

        /// <summary>
        ///     Adds an item to the <see cref="T:System.Collections.Generic.ICollection{T}" />.
        /// </summary>
        /// <param name="item"> The object to add to the <see cref="T:System.Collections.Generic.ICollection{T}" />. </param>
        public void Add(KeyValuePair<string, TElement> item) => base.Add(item.Key, item.Value);

        /// <summary>
        ///     Adds the specified element to the dictionary, using the name of the object as a the key.
        /// </summary>
        /// <param name="key"> The key identifying the value to add.. </param>
        /// <param name="element"> The element to add to the dictionary. </param>
        public new void Add(string key, TElement element)
        {
            base.Add(key, element);
        }

        /// <summary>
        ///     Adds a new value with the key specified, copying the properties and elements from the value give,
        ///     returning the new value.
        /// </summary>
        /// <param name="item"> The object to add to the <see cref="T:System.Collections.Generic.ICollection{T}" />. </param>
        /// <returns> The newly added element. </returns>
        /// <remarks>
        ///     Used to add objects and collections that have been constructed externally using alternate implementations.
        /// </remarks>
        public void AddCopy(KeyValuePair<string, TElement> item) => AddCopy(item.Key, item.Value);

        /// <summary>
        ///     Determines whether this instance contains the object.
        /// </summary>
        /// <param name="element"> The element for which to check. </param>
        /// <returns> <see langword="true" /> if the dictionary contains the specified element; otherwise, <see langword="false" />. </returns>
        public bool Contains(TElement element) => ContainsValue(element);

        /// <summary>
        ///     Copies the elements of the <see cref="ICollection{T}" /> to an <see cref="System.Array" />, starting at
        ///     a particular <see cref="System.Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="System.Array" /> that is the destination of the elements copied from
        ///     <see cref="ICollection{T}" />. The <see cref="System.Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex"> The zero-based index in <paramref name="array" /> at which copying begins. </param>
        public void CopyTo(KeyValuePair<string, TElement>[] array, int arrayIndex)
        {
            EnforceDisposed();

            Lock.EnterReadLock();
            try
            {
                OrderedItems.Select(e => new KeyValuePair<string, TElement>(e.Key, e.Value)).ToArray().CopyTo(array, arrayIndex);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

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
                return OrderedItems.Select(e => new KeyValuePair<string, TElement>(e.Key, e.Value)).GetEnumerator();
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the <see cref="ICollection{T}" />.
        /// </summary>
        /// <param name="item"> The object to remove from the <see cref="ICollection{T}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> was successfully removed from the
        ///     <see cref="ICollection{T}" />; otherwise, <see langword="false" />. This method also returns
        ///     <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="ICollection{T}" />.
        /// </returns>
        public bool Remove(KeyValuePair<string, TElement> item)
        {
            return base.Remove(item.Key);
        }

        /// <summary>
        ///     Adds the specified item to the collection, using the key specified.
        /// </summary>
        /// <param name="key"> The key used to identify the item to add. Must be unique. </param>
        /// <param name="value"> The value to assign to the value. </param>
        void IDictionary<string, TElement>.Add(string key, TElement value) => base.Add(key, value);

        /// <summary>
        ///     Adds a new value with the key specified, returning the new value.
        /// </summary>
        /// <param name="key"> The key identifying the value to add. </param>
        /// <returns> The newly added value. </returns>
        TElement IConfigurationDictionary<TElement>.AddNew(string key) => AddNew(key);

        /// <summary>
        ///     Determines whether this instance contains the object given.
        /// </summary>
        /// <param name="item"> The object to locate in the <see cref="ICollection{TElement}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> is found in the
        ///     <see cref="ICollection{TElement}" />; otherwise, <see langword="false" />.
        /// </returns>
        bool ICollection<KeyValuePair<string, TElement>>.Contains(KeyValuePair<string, TElement> item) => ContainsKey(item.Key);

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
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