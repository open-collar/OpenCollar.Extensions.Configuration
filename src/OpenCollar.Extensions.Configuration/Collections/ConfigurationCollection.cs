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
using System.Linq;
using Microsoft.Extensions.Configuration;
using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A collection of <see cref="IConfigurationObject"> configuration objects </see> that notifies when an element
    ///     is added or removed.
    /// </summary>
    /// <typeparam name="TElement"> The type of the element. </typeparam>
    /// <seealso cref="ConfigurationDictionaryBase{T,T}" />
    /// <seealso cref="IConfigurationCollection{TElement}" />
    [System.Diagnostics.DebuggerDisplay("ConfigurationCollection[{Count}] ({GetPath()})")]
    public class ConfigurationCollection<TElement> : ConfigurationDictionaryBase<int, TElement>, IConfigurationCollection<TElement>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationCollection{TElement}" /> class.
        /// </summary>
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        /// <param name="configurationRoot">
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </param>
        /// <param name="elements"> The elements with which to initialize to the collection. </param>
        public ConfigurationCollection(IConfigurationParent? parent, PropertyDef propertyDef, IConfigurationRoot configurationRoot, IEnumerable<TElement>? elements) : base(parent, propertyDef, configurationRoot, GetIndexedElements(elements))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationCollection{TElement}" /> class.
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
        public ConfigurationCollection(IConfigurationParent? parent, PropertyDef propertyDef, IConfigurationRoot configurationRoot, params TElement[]? elements) : base(parent, propertyDef, configurationRoot, GetIndexedElements(elements))
        {
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="ICollection{TElement}" /> is read-only.
        /// </summary>
        /// <value> <see langword="true" /> if this collection is read-only; otherwise, <see langword="false" />. </value>
        protected override bool InnerIsReadOnly
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
        ///     Adds a new value with the key specified, copying the properties and elements from the value give,
        ///     returning the new value.
        /// </summary>
        /// <param name="value"> The value to copy. </param>
        /// <returns> The newly added element. </returns>
        /// <remarks>
        ///     Used to add objects and collections that have been constructed externally using alternate implementations.
        /// </remarks>
        public TElement AddCopy(TElement value) => AddCopy(Count, value);

        /// <summary>
        ///     Adds a new value with the key specified, returning the new value.
        /// </summary>
        /// <returns> The newly added element. </returns>
        public TElement AddNew() => AddNew(Count);

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
                if(UniversalComparer.Equals(element, item))
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

            if(PropertyDef.ElementImplementation.ImplementationKind != ImplementationKind.Naive)
            {
                if(!ReferenceEquals(item, null))
                {
                    if(PropertyDef.ElementImplementation.ImplementationType != item.GetType())
                    {
                        throw new TypeMismatchException($"Expected object of type {PropertyDef.ElementImplementation.ImplementationType.FullName}.", GetPath());
                    }
                }
            }

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

            var n = 0;
            var events = new List<System.Collections.Specialized.NotifyCollectionChangedEventArgs>();
            Lock.EnterWriteLock();
            try
            {
                var entries = new KeyValuePair<int, Element<int, TElement>>[InnerCount];
                InnerCopyTo(entries, 0);

                var list = new List<Element<int, TElement>>(entries.Select(k => k.Value));
                DisableCollectionChangedEvents = true;
                try
                {
                    list.Insert(index, new Element<int, TElement>(PropertyDef, this, index) { Value = item });
                }
                finally
                {
                    DisableCollectionChangedEvents = false;
                }
                events.Add(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Add, item, index));
                foreach(var element in list.ToArray())
                {
                    if(n > index)
                    {
                        list[n].Key = n;
                        events.Add(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Move, list[n], n, n - 1));
                    }
                    ++n;
                }
                Replace(list);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            foreach(var args in events)
            {
                OnCollectionChanged(args);
            }
        }

        /// <summary>
        ///     Inserts an iten at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the location at which the item should be inserted. </param>
        /// <param name="item"> The item to insert. </param>
        public TElement InsertCopy(int index, TElement item)
        {
            TElement copy;

            switch(PropertyDef.ElementImplementation.ImplementationKind)
            {
                case ImplementationKind.ConfigurationCollection:
                    if(ReferenceEquals(item, null))
                    {
                        copy = default;
                    }
                    else
                    {
                        copy = (TElement)Activator.CreateInstance(PropertyDef.ElementImplementation.ImplementationType, this, PropertyDef, ConfigurationRoot, item);
                    }
                    break;

                case ImplementationKind.ConfigurationDictionary:
                    if(ReferenceEquals(item, null))
                    {
                        copy = default;
                    }
                    else
                    {
                        copy = (TElement)Activator.CreateInstance(PropertyDef.ElementImplementation.ImplementationType, this, PropertyDef, ConfigurationRoot, item);
                    }
                    break;

                case ImplementationKind.ConfigurationObject:
                    if(ReferenceEquals(item, null))
                    {
                        copy = default;
                    }
                    else
                    {
                        var clone = Activator.CreateInstance(PropertyDef.ElementImplementation.ImplementationType, ConfigurationRoot, this);
                        ((ConfigurationObjectBase<TElement>)clone).Clone(item);
                        copy = (TElement)clone;
                    }
                    break;

                default:
                    copy = item;
                    break;
            }

            Insert(index, copy);

            return copy;
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the <see cref="ICollection{TElement}" />.
        /// </summary>
        /// <param name="item"> The object to remove from the <see cref="ICollection{TElement}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> was successfully removed from the
        ///     <see cref="ICollection{TElement}" />; otherwise, <see langword="false" />. This method also returns
        ///     <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="ICollection{TElement}" />.
        /// </returns>
        public override bool Remove(TElement item)
        {
            var index = IndexOf(item);
            if(index >= 0)
            {
                return Remove(index);
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
                Reindex(index, (n, e) => n);
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
            Reindex(index, (n, e) => n);
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)Values).GetEnumerator();
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