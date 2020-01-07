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

using System;
using System.Collections.Generic;

using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// A collection of <see cref="IConfigurationObject"> configuration objects </see> that notifies when an element
    /// is added or removed.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <seealso cref="ConfigurationDictionaryBase{T,T}" />
    /// <seealso cref="IConfigurationCollection{TElement}" />
    internal class ConfigurationCollection<TElement> : ConfigurationDictionaryBase<int, TElement>, IConfigurationCollection<TElement>
        where TElement : IConfigurationObject
    {
        /// <summary>Initializes a new instance of the <see cref="ConfigurationCollection{TElement}" /> class.</summary>
        /// <param name="propertyDef">The definition of the property defined by this object.</param>
        /// <param name="elements">The elements with which to initialize to the collection.</param>
        public ConfigurationCollection(PropertyDef propertyDef, IEnumerable<TElement>? elements) : base(propertyDef, GetIndexedElements(elements))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ConfigurationCollection{TElement}" /> class.</summary>
        /// <param name="propertyDef">The definition of the property defined by this object.</param>
        /// <param name="elements">A parameter array containing the elements with which to initialize to the collection.</param>
        public ConfigurationCollection(PropertyDef propertyDef, params TElement[]? elements) : base(propertyDef, GetIndexedElements(elements))
        {
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="ICollection{TElement}" /> is read-only.
        /// </summary>
        /// <value> <see langword="true" /> if this collection is read-only; otherwise, <see langword="false" />. </value>
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether to set values using the key first.
        /// </summary>
        /// <value> <see langword="true" /> if set value using key first; otherwise to value first, <see langword="false" />. </value>
        protected override bool SetValueUsingKeyFirst
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Gets or sets the <typeparamref name="TElement" /> at the specified index.
        /// </summary>
        /// <value> The <typeparamref name="TElement" /> at the index specified. </value>
        /// <param name="index"> The index of the item to get or set. </param>
        /// <returns> </returns>
        public new TElement this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
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
        public IEnumerator<TElement> GetEnumerator() => Values.GetEnumerator();

        /// <summary>
        ///     Finds the index of the first element in the collection that equals the item provided.
        /// </summary>
        /// <param name="item"> The item to find. </param>
        /// <returns>
        ///     The zero-based index of the first matching item or a value less than zero if no match is found.
        /// </returns>
        public int IndexOf(TElement item)
        {
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
        public void Insert(int index, TElement item)
        {
            EnforceDisposed();

            if(index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"'{nameof(index)}' must be greater than or equal to zero.");
            }
            if(index > InnerCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"'{nameof(index)}' must be less than or equal to the number of items in the collection.");
            }

            if(index == Count)
            {
                Add(item);
                return;
            }

            Lock.EnterWriteLock();
            try
            {
                var entries = new KeyValuePair<int, TElement>[InnerCount];
                InnerCopyTo(entries, 0);

                var list = new List<KeyValuePair<int, TElement>>(entries);
                list.Insert(index, new KeyValuePair<int, TElement>(index, item));
                var n = 0;
                foreach(var element in list.ToArray())
                {
                    if(n > index)
                    {
                        list[n] = new KeyValuePair<int, TElement>(n, element.Value);
                    }
                    ++n;
                }
                Replace(list);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the <see cref="ICollection{TElement}" />.
        /// </summary>
        /// <param name="item"> The object to remove from the <see cref="ICollection{TElement}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> was successfully removed from the
        ///     <see cref="ICollection{TElement}" />; otherwise, <see langword="false" />.
        ///     This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the
        ///     original <see cref="ICollection{TElement}" />.
        /// </returns>
        public override bool Remove(TElement item)
        {
            var index = IndexOf(item);
            if(index >= 0)
            {
                Remove(index);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes the element with the specified key from the <see cref="IDictionary{T,T}" />.
        /// </summary>
        /// <param name="index"> The zero-based index of the element to remove. </param>
        /// <returns>
        ///     <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.
        ///     This method also returns <see langword="false" /> if <paramref name="key" /> was not found in the
        ///     original <see cref="IDictionary{T,T}" />.
        /// </returns>
        public override bool Remove(int index)
        {
            if(base.Remove(index))
            {
                Reindex(index);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes the item at the zero-based index specifiedt.
        /// </summary>
        /// <param name="index"> The index of the item to remove. </param>
        public void RemoveAt(int index)
        {
            base.Remove(index);
            Reindex(index);
        }

        /// <summary>
        ///     Reindexes the items after the specified index after removing an item.
        /// </summary>
        /// <param name="removedIndex"> Index of the removed item. </param>
        private void Reindex(int removedIndex)
        {
            if(removedIndex >= Count)
            {
                // The last item was removed, we don't need to do anything.
                return;
            }

            // Everything from the removed item onwards will need to be moved back one.
            for(var n = removedIndex; n < Count; ++n)
            {
                var item = base[n + 1];
                Remove(n + 1);
                Add(new KeyValuePair<int, TElement>(n, item));
            }
        }
    }
}