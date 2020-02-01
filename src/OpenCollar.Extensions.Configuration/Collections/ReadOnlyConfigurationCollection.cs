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

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A read-only collection of <see cref="IConfigurationObject"> configuration objects </see> that notifies when
    ///     an element is added or removed.
    /// </summary>
    /// <typeparam name="TElement"> The type of the element. </typeparam>
    internal sealed class ReadOnlyConfigurationCollection<TElement> : ConfigurationDictionaryBase<int, TElement>, IReadOnlyCollection<TElement>, IConfigurationCollection<TElement>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyConfigurationCollection{TElement}" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        /// <param name="configurationRoot">
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </param>
        /// <param name="elements"> The elements with which to initialize to the collection. </param>
        public ReadOnlyConfigurationCollection(PropertyDef propertyDef, IConfigurationRoot configurationRoot, IEnumerable<TElement>? elements) : base(propertyDef, configurationRoot, GetIndexedElements(elements))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyConfigurationCollection{TElement}" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        /// <param name="configurationRoot">
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </param>
        /// <param name="elements">
        ///     A parameter array containing the elements with which to initialize to the collection.
        /// </param>
        public ReadOnlyConfigurationCollection(PropertyDef propertyDef, IConfigurationRoot configurationRoot, params TElement[]? elements) : base(propertyDef, configurationRoot, GetIndexedElements(elements))
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
        ///     Gets or sets the <see cref="TElement" /> at the specified index.
        /// </summary>
        /// <value> The <see cref="TElement" />. </value>
        /// <param name="index"> The index. </param>
        /// <returns> </returns>
        TElement IList<TElement>.this[int index]
        {
            get
            {
                return base[index].Value;
            }
            set
            {
                base[index].Value = value;
            }
        }

        /// <summary>
        ///     Adds the specified item to the end of the collection..
        /// </summary>
        /// <param name="item"> The item to add. </param>
        public void Add(TElement item) => Add(Count, item);

        /// <summary>
        ///     Determines whether this instance contains the object.
        /// </summary>
        /// <param name="item"> The item for which to check. </param>
        /// <returns> <see langword="true" /> if the collection contains the specified item; otherwise, <see langword="false" />. </returns>
        public bool Contains(TElement item) => ContainsValue(item);

        /// <summary>
        ///     Copies the contents of the collection to an array.
        /// </summary>
        /// <param name="array"> The array to which to copy the contents of the collection. </param>
        /// <param name="arrayIndex">
        ///     The index of the first location in the array to which to copy the collection contents.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="arrayIndex" /> must be at least zero. or <paramref name="array" /> is not large enough
        ///     to hold the contents of this collection (if data is copied to the location specified by <paramref name="arrayIndex" />.
        /// </exception>
        public void CopyTo(TElement[] array, int arrayIndex)
        {
            EnforceDisposed();

            if(arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, $"'{nameof(arrayIndex)}' must be at least zero.");
            }
            if((arrayIndex + Count) > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(array), array, $"'{nameof(array)}' is not large enough to hold the contents of this collection (if data is copied to the location specified by '{nameof(arrayIndex)}'.");
            }

            foreach(var item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        /// <summary>
        ///     Gets the enumerator for the values in this collection.
        /// </summary>
        /// <returns> The enumerator for the values in this collection. </returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            EnforceDisposed();

            return Values.GetEnumerator();
        }

        /// <summary>
        ///     Finds the index of the first element in the collection that equals the item provided.
        /// </summary>
        /// <param name="item"> The item to find. </param>
        /// <returns>
        ///     The zero-based index of the first matching item or a value less than zero if no match is found.
        /// </returns>
        public int IndexOf(TElement item)
        {
            EnforceDisposed();

            var n = 0;
            foreach(var element in this)
            {
                if(Equals(element, item))
                {
                    return n;
                }
                ++n;
            }

            return -1;
        }

        /// <summary>
        ///     Inserts an iten at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the location at which the item should be inserted. </param>
        /// <param name="item"> The item to insert. </param>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        public void Insert(int index, TElement item)
        {
            EnforceDisposed();
            throw new NotImplementedException("This collection is read-only.");
        }

        /// <summary>
        ///     Removes the item at the zero-based index specifiedt.
        /// </summary>
        /// <param name="index"> The index of the item to remove. </param>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        public void RemoveAt(int index)
        {
            EnforceDisposed();
            throw new NotImplementedException("This collection is read-only.");
        }

        /// <summary>
        ///     Converts the string given to the key.
        /// </summary>
        /// <param name="key"> The key to convert, as a string. </param>
        /// <returns> Returns the key converted to the correct type. </returns>
        internal override int ConvertStringToKey(string key)
        {
            return int.Parse(key, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}