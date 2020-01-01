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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace OpenCollar.Extensions.Configuration.Collections
{
    /// <summary>
    ///     A base class provide the functionality used by both dictionaries and collections.
    /// </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    /// <typeparam name="TElement"> The type of the element. </typeparam>
    /// <seealso cref="OpenCollar.Extensions.Configuration.IConfigurationObject" />
    /// <seealso cref="System.Collections.Generic.IDictionary{TKey, TElement}" />
    /// <seealso cref="System.Collections.Specialized.INotifyCollectionChanged" />
    public abstract class ConfigurationDictionaryBase<TKey, TElement> : Disposable, IEnumerable, INotifyCollectionChanged, IConfigurationObject
        where TElement : IConfigurationObject
    {
        /// <summary>
        ///     A thread-safe dictionary containing the elements of the dictionary recorded against the keys supplied.
        /// </summary>
        private readonly Dictionary<TKey, TElement> _items = new Dictionary<TKey, TElement>();

        /// <summary>
        ///     An ordered list of the elements in the collection.
        /// </summary>
        private readonly List<KeyValuePair<TKey, TElement>> _orderedItems = new List<KeyValuePair<TKey, TElement>>();

        /// <summary>
        ///     Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="System.Collections.Generic.ICollection{T}" />.
        /// </summary>
        public int Count
        {
            get
            {
                EnforceDisposed();

                Lock.EnterReadLock();
                try
                {
                    return _items.Count;
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this object has any properties with unsaved changes.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if this object has any properties with unsaved changes; otherwise,
        ///     <see langword="false" /> .
        /// </value>
        public bool IsDirty
        {
            get
            {
                EnforceDisposed();

                Lock.EnterReadLock();
                try
                {
                    return _items.Values.Any(i => i.IsDirty);
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="System.Collections.Generic.ICollection{T}" /> is read-only.
        /// </summary>
        public abstract bool IsReadOnly { get; }

        /// <summary>
        ///     Gets an <see cref="System.Collections.Generic.ICollection{T}" /> containing the keys of the <see cref="System.Collections.Generic.IDictionary{T,T}" />.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                EnforceDisposed();

                Lock.EnterReadLock();
                try
                {
                    // Use the order items to ensure the keys are in the correct order.
                    return _orderedItems.Select(i => i.Key).ToArray();
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        ///     Gets an <see cref="System.Collections.Generic.ICollection{T}" /> containing the values in the <see cref="System.Collections.Generic.IDictionary{T,T}" />.
        /// </summary>
        public ICollection<TElement> Values
        {
            get
            {
                EnforceDisposed();

                Lock.EnterReadLock();
                try
                {
                    // Use the order items to ensure the values are in the correct order.
                    return _orderedItems.Select(v => v.Value).ToArray();
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        ///     Gets the lock object used to control concurrent access to the collection.
        /// </summary>
        /// <value> The lock object used to control concurrent access to the collection. </value>
        protected ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        ///     Gets the items in the dictionary as an ordered, read-only list.
        /// </summary>
        /// <value> The items in the dictionary as an ordered, read-only list. </value>
        protected IReadOnlyList<KeyValuePair<TKey, TElement>> OrderedItems { get { return _orderedItems; } }

        /// <summary>
        ///     Gets or sets the item with the specified key.
        /// </summary>
        /// <value> The item to get or set. </value>
        /// <param name="key"> The key identifying the element to get or set. </param>
        /// <returns> The element specified by <paramref name="key" />. </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="key" /> did not identify a valid element.
        /// </exception>
        public TElement this[TKey key]
        {
            get
            {
                EnforceDisposed();

                if(_items.TryGetValue(key, out var value))
                {
                    return value;
                }

                throw new ArgumentOutOfRangeException(nameof(key), "'key' did not identify a valid element.");
            }
            set
            {
                EnforceDisposed();

                Lock.EnterUpgradeableReadLock();
                try
                {
                    if(_items.ContainsKey(key))
                    {
                        var foundItem = _items[key];
                        if(Equals(value, foundItem))
                        {
                            return;
                        }

                        Lock.EnterWriteLock();
                        try
                        {
                            _items[key] = value;
                            var n = 0;
                            foreach(var element in _orderedItems)
                            {
                                if(Equals(element.Value, foundItem))
                                {
                                    _orderedItems[n] = new KeyValuePair<TKey, TElement>(key, value);
                                    break;
                                }
                                ++n;
                            }
                        }
                        finally
                        {
                            Lock.ExitWriteLock();
                        }
                    }
                    else
                    {
                        Lock.EnterWriteLock();
                        try
                        {
                            _items.Add(key, value);
                            _orderedItems.Add(new KeyValuePair<TKey, TElement>(key, value));
                            OnPropertyChanged(nameof(Count));
                        }
                        finally
                        {
                            Lock.ExitWriteLock();
                        }
                    }

                    // TODO: add change detection and notification.
                    Debug.Assert(_orderedItems.Count == _items.Count);
                }
                finally
                {
                    Lock.ExitUpgradeableReadLock();
                }
            }
        }

        /// <summary>
        ///     Adds an element with the provided key and value to the <see cref="System.Collections.Generic.IDictionary{T,T}" />.
        /// </summary>
        /// <param name="key"> The object to use as the key of the element to add. </param>
        /// <param name="value"> The object to use as the value of the element to add. </param>
        public void Add(TKey key, TElement value)
        {
            Add(new KeyValuePair<TKey, TElement>(key, value));
        }

        /// <summary>
        ///     Adds an item to the <see cref="System.Collections.Generic.ICollection{T}" />.
        /// </summary>
        /// <param name="item"> The object to add to the <see cref="System.Collections.Generic.ICollection{T}" />. </param>
        public void Add(KeyValuePair<TKey, TElement> item)
        {
            EnforceDisposed();

            Lock.EnterUpgradeableReadLock();
            try
            {
                if(_items.TryAdd(item.Key, item.Value))
                {
                    Lock.EnterWriteLock();
                    try
                    {
                        _orderedItems.Add(item);
                        OnPropertyChanged(nameof(Count));
                        return;
                    }
                    finally
                    {
                        Lock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                Lock.ExitUpgradeableReadLock();
            }

            throw new ArgumentException("An item with the same key has already been added.", nameof(item));
        }

        /// <summary>
        ///     Removes all items from the <see cref="System.Collections.Generic.ICollection{T}" />.
        /// </summary>
        public void Clear()
        {
            EnforceDisposed();

            InternalClear();
        }

        /// <summary>
        ///     Determines whether this instance contains the object.
        /// </summary>
        /// <param name="item"> The object to locate in the <see cref="System.Collections.Generic.ICollection{T}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> is found in the
        ///     <see cref="System.Collections.Generic.ICollection{T}" />; otherwise, <see langword="false" />.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TElement> item)
        {
            EnforceDisposed();

            Lock.EnterReadLock();
            try
            {
                return _items.ContainsKey(item.Key);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Determines whether the <see cref="System.Collections.Generic.IDictionary{T,T}" /> contains an element
        ///     with the specified key.
        /// </summary>
        /// <param name="key"> The key to locate in the <see cref="System.Collections.Generic.IDictionary{T,T}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if the <see cref="System.Collections.Generic.IDictionary{T,T}" /> contains an
        ///     element with the key; otherwise, <see langword="false" />.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            EnforceDisposed();

            Lock.EnterReadLock();
            try
            {
                return _items.ContainsKey(key);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Copies the elements of the <see cref="System.Collections.Generic.ICollection{T}" /> to an
        ///     <see cref="System.Array" />, starting at a particular <see cref="System.Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="System.Array" /> that is the destination of the elements copied from
        ///     <see cref="System.Collections.Generic.ICollection{T}" />. The <see cref="System.Array" /> must have
        ///     zero-based indexing.
        /// </param>
        /// <param name="arrayIndex"> The zero-based index in <paramref name="array" /> at which copying begins. </param>
        public void CopyTo(KeyValuePair<TKey, TElement>[] array, int arrayIndex)
        {
            EnforceDisposed();

            Lock.EnterReadLock();
            try
            {
                _orderedItems.CopyTo(array, arrayIndex);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        ///// <summary>
        /////     Returns an enumerator that iterates through the collection.
        ///// </summary>
        ///// <returns> An enumerator that can be used to iterate through the collection. </returns>
        //public IEnumerator<KeyValuePair<TKey, TElement>> GetEnumerator()
        //{
        //    EnforceDisposed();

        //    Lock.EnterReadLock();
        //    try
        //    {
        //        return _orderedItems.GetEnumerator();
        //    }
        //    finally
        //    {
        //        Lock.ExitReadLock();
        //    }
        //}

        /// <summary>
        ///     Loads all of the properties from the configuration sources, overwriting any unsaved changes.
        /// </summary>
        /// <exception cref="NotImplementedException"> </exception>
        public void Reload() => throw new NotImplementedException();

        /// <summary>
        ///     Removes the element with the specified key from the <see cref="System.Collections.Generic.IDictionary{T,T}" />.
        /// </summary>
        /// <param name="key"> The key of the element to remove. </param>
        /// <returns>
        ///     <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.
        ///     This method also returns <see langword="false" /> if <paramref name="key" /> was not found in the
        ///     original <see cref="System.Collections.Generic.IDictionary{T,T}" />.
        /// </returns>
        public virtual bool Remove(TKey key)
        {
            EnforceDisposed();

            Lock.EnterWriteLock();
            try
            {
                if(_items.Remove(key))
                {
                    var removed = _orderedItems.First(i => i.Key.Equals(key));
                    if(!_orderedItems.Remove(removed))
                    {
                        Debug.Assert(false, "A matching item must always be found.");
                    }

                    OnPropertyChanged(nameof(Count));
                    return true;
                }

                return false;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the <see cref="System.Collections.Generic.ICollection{T}" />.
        /// </summary>
        /// <param name="item"> The object to remove from the <see cref="System.Collections.Generic.ICollection{T}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> was successfully removed from the
        ///     <see cref="System.Collections.Generic.ICollection{T}" />; otherwise, <see langword="false" />. This
        ///     method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="System.Collections.Generic.ICollection{T}" />.
        /// </returns>
        public bool Remove(KeyValuePair<TKey, TElement> item)
        {
            EnforceDisposed();

            Lock.EnterWriteLock();
            try
            {
                if(_items.Remove(item.Key, out var removed))
                {
                    if(!_orderedItems.Remove(item))
                    {
                        Debug.Assert(false, "A matching item must always be found.");
                    }

                    OnPropertyChanged(nameof(Count));
                    return true;
                }

                return false;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the <see cref="System.Collections.Generic.ICollection{T}" />.
        /// </summary>
        /// <param name="item"> The object to remove from the <see cref="System.Collections.Generic.ICollection{T}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> was successfully removed from the
        ///     <see cref="System.Collections.Generic.ICollection{T}" />; otherwise, <see langword="false" />. This
        ///     method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="System.Collections.Generic.ICollection{T}" />.
        /// </returns>
        public virtual bool Remove(TElement item)
        {
            EnforceDisposed();
            Lock.EnterWriteLock();
            try
            {
                foreach(var element in _items.ToArray())
                {
                    if(Equals(element.Value, item))
                    {
                        if(!_items.Remove(element.Key, out var removedElement))
                        {
                            System.Diagnostics.Debug.Assert(false, "We assume the element can be removed if it can be found.");
                        }

                        var n = 0;
                        foreach(var orderedElement in _orderedItems.ToArray())
                        {
                            if(Equals(orderedElement.Value, item))
                            {
                                _orderedItems.RemoveAt(n);
                                OnPropertyChanged(nameof(Count));
                                return true;
                            }
                            ++n;
                        }
                        System.Diagnostics.Debug.Assert(false, "We assume the element can be removed from both collections.");
                    }
                }

                return false;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Saves this current values for each property back to the configuration sources.
        /// </summary>
        /// <exception cref="NotImplementedException"> </exception>
        public void Save() => throw new NotImplementedException();

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key"> The key whose value to get. </param>
        /// <param name="value">
        ///     When this method returns, the value associated with the specified key, if the key is found; otherwise,
        ///     the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the object that implements
        ///     <see cref="System.Collections.Generic.IDictionary{T,T}" /> contains an element with the specified key;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public bool TryGetValue(TKey key, out TElement value)
        {
            EnforceDisposed();

            Lock.EnterReadLock();
            try
            {
                return _items.TryGetValue(key, out value);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            EnforceDisposed();

            Lock.EnterReadLock();
            try
            {
                return ((IEnumerable)_orderedItems).GetEnumerator();
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to
        ///     release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                InternalClear();
                Lock.Dispose();
            }
        }

        /// <summary>
        ///     Removes all items from the <see cref="System.Collections.Generic.ICollection{T}" />.
        /// </summary>
        private void InternalClear()
        {
            Lock.EnterWriteLock();
            try
            {
                _items.Clear();
                _orderedItems.Clear();
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            OnPropertyChanged(nameof(Count));
        }

        /// <summary>
        ///     Called when the collection has changed.
        /// </summary>
        /// <param name="action"> The action that occurred. </param>
        private void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            var eventHandler = CollectionChanged;
            if(ReferenceEquals(eventHandler, null))
            {
                return;
            }

            var callbacks = eventHandler.GetInvocationList();

            if(callbacks.Length <= 0)
            {
                return;
            }

            var args = new NotifyCollectionChangedEventArgs(action);

            foreach(var callback in callbacks)
            {
                callback.DynamicInvoke(this, args);
            }
        }

        /// <summary>
        ///     Called when a property is to be changed.
        /// </summary>
        /// <typeparam name="T"> The type of the property. </typeparam>
        /// <param name="field"> The field to which the value is to be assigned. </param>
        /// <param name="value"> The value to assign. </param>
        /// <param name="propertyName"> The name of the property that has changed. </param>
        /// <remarks> Raises the <see cref="PropertyChanged" /> event if the value has changed. </remarks>
        private void OnPropertyChanged<T>(ref T field, T value, string propertyName)
        {
            if(Equals(field, value))
            {
                return;
            }

            field = value;

            OnPropertyChanged(propertyName);
        }

        /// <summary>
        ///     Called when a property is to be changed.
        /// </summary>
        /// <param name="propertyName"> The name of the property that has changed. </param>
        /// <remarks> Raises the <see cref="PropertyChanged" /> event. </remarks>
        private void OnPropertyChanged(string propertyName)
        {
            if(IsDisposed)
            {
                return;
            }

            var eventHandler = PropertyChanged;
            if(ReferenceEquals(eventHandler, null))
            {
                return;
            }

            var callbacks = eventHandler.GetInvocationList();

            if(callbacks.Length <= 0)
            {
                return;
            }

            var args = new PropertyChangedEventArgs(propertyName);

            foreach(var callback in callbacks)
            {
                callback.DynamicInvoke(this, args);
            }
        }
    }
}