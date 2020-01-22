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
using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration.Collections
{
    /// <summary>
    ///     A base class provide the functionality used by both dictionaries and collections.
    /// </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    /// <typeparam name="TElement"> The type of the element. </typeparam>
    /// <seealso cref="IConfigurationObject" />
    /// <seealso cref="IDictionary{TKey, TElement}" />
    /// <seealso cref="INotifyCollectionChanged" />
    internal abstract class ConfigurationDictionaryBase<TKey, TElement> : Disposable, IEnumerable, INotifyCollectionChanged, IConfigurationObject
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
        ///     Initializes a new instance of the <see cref="ConfigurationDictionaryBase{TKey, TElement}" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        /// <param name="configurationRoot">
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </param>
        protected ConfigurationDictionaryBase(PropertyDef propertyDef, IConfigurationRoot configurationRoot)
        {
            PropertyDef = propertyDef;
            ConfigurationRoot = configurationRoot;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationDictionaryBase{TKey, TElement}" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        /// <param name="elements"> The elements with which to initialize to the collection. </param>
        /// <param name="configurationRoot">
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </param>
        protected ConfigurationDictionaryBase(PropertyDef propertyDef, IConfigurationRoot configurationRoot, IEnumerable<KeyValuePair<TKey, TElement>> elements) : this(propertyDef, configurationRoot)
        {
            PropertyDef = propertyDef;

            foreach(var element in elements)
            {
                _items.Add(element.Key, element.Value);
                _orderedItems.Add(element);
            }
        }

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="ICollection{T}" />.
        /// </summary>
        /// <value> The number of elements contained in the <see cref="ICollection{T}" />. </value>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
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
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
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
        ///     Gets a value indicating whether the <see cref="ICollection{T}" /> is read-only.
        /// </summary>
        public abstract bool IsReadOnly
        {
            get;
        }

        /// <summary>
        ///     Gets an <see cref="ICollection{T}" /> containing the keys of the <see cref="IDictionary{T,T}" />.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
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
        ///     Gets the definition of this property object.
        /// </summary>
        /// <value> The definition of this property object. </value>
        public PropertyDef PropertyDef
        {
            get;
        }

        /// <summary>
        ///     Gets an <see cref="ICollection{T}" /> containing the values in the <see cref="IDictionary{T,T}" />.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
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
        ///     Gets the configuration root service from which values are read or to which all values will be written.
        /// </summary>
        /// <value>
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </value>
        internal IConfigurationRoot ConfigurationRoot
        {
            get;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether events are raised for changes.
        /// </summary>
        /// <value> <see langword="true" /> if change events are disabled; otherwise, <see langword="false" />. </value>
        protected bool DisableEvents
        {
            get; set;
        }

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="ICollection{T}" />.
        /// </summary>
        /// <value> The number of elements contained in the <see cref="ICollection{T}" />. </value>
        /// <remarks> Assumes that the caller already holds a read or write lock. </remarks>
        protected int InnerCount
        {
            get
            {
                return _items.Count;
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
        protected IReadOnlyList<KeyValuePair<TKey, TElement>> OrderedItems
        {
            get
            {
                return _orderedItems;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether to set values using the key first.
        /// </summary>
        /// <value> <see langword="true" /> if set value using key first; otherwise to value first, <see langword="false" />. </value>
        protected virtual bool SetValueUsingKeyFirst
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets or sets the item with the specified key.
        /// </summary>
        /// <value> The item to get or set. </value>
        /// <param name="key"> The key identifying the element to get or set. </param>
        /// <returns> The element specified by <paramref name="key" />. </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="key" /> did not identify a valid element.
        /// </exception>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
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

                throw new ArgumentOutOfRangeException(nameof(key), $"'{nameof(key)}' did not identify a valid element.");
            }
            set
            {
                EnforceDisposed();

                EnforceReadOnly();

                if(SetValueUsingKeyFirst)
                {
                    SetValueByKey(key, value);
                }
                else
                {
                    SetValueByValue(key, value);
                }
            }
        }

        /// <summary>
        ///     Adds an element with the provided key and value to the <see cref="IDictionary{T,T}" />.
        /// </summary>
        /// <param name="key"> The object to use as the key of the element to add. </param>
        /// <param name="value"> The object to use as the value of the element to add. </param>
        public virtual void Add(TKey key, TElement value)
        {
            Add(new KeyValuePair<TKey, TElement>(key, value));
        }

        /// <summary>
        ///     Adds an item to the <see cref="ICollection{T}" />.
        /// </summary>
        /// <param name="item"> The object to add to the <see cref="ICollection{T}" />. </param>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        public void Add(KeyValuePair<TKey, TElement> item)
        {
            EnforceDisposed();
            EnforceReadOnly();

            NotifyCollectionChangedEventArgs? args = null;

            Lock.EnterUpgradeableReadLock();
            try
            {
                if(_items.TryAdd(item.Key, item.Value))
                {
                    Lock.EnterWriteLock();
                    try
                    {
                        _orderedItems.Add(item);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item.Value);
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

            if(!ReferenceEquals(args, null))
            {
                OnCollectionChanged(args);
                OnPropertyChanged(nameof(Count));
                return;
            }

            throw new ArgumentException("An item with the same key has already been added.", nameof(item));
        }

        /// <summary>
        ///     Removes all items from the <see cref="ICollection{T}" />.
        /// </summary>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        public void Clear()
        {
            EnforceDisposed();
            EnforceReadOnly();

            InternalClear();

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///     Determines whether the <see cref="IDictionary{T,T}" /> contains an element with the specified key.
        /// </summary>
        /// <param name="key"> The key to locate in the <see cref="IDictionary{T,T}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if the <see cref="IDictionary{T,T}" /> contains an element with the key;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
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
        ///     Copies the elements of the <see cref="ICollection{T}" /> to an <see cref="Array" />, starting at a
        ///     particular <see cref="Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="Array" /> that is the destination of the elements copied from
        ///     <see cref="ICollection{T}" />. The <see cref="Array" /> must have zero-based indexing.
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

        /// <summary>
        ///     Recursively deletes all of the properties from the configuration sources.
        /// </summary>
        /// <exception cref="NotImplementedException"> </exception>
        public void Delete() => throw new NotImplementedException();

        /// <summary>
        ///     Loads all of the properties from the configuration sources, overwriting any unsaved changes.
        /// </summary>
        /// <exception cref="NotImplementedException"> </exception>
        public void Reload()
        {
            // Iterate across all of the elements in the path and then delete those not in the dictionary, and then
            // insert those that have been added.
            var section = ConfigurationRoot.GetSection(PropertyDef.Path);
            var existingValues = section.GetChildren().Select(s => new KeyValuePair<TKey, IConfigurationSection>(ConvertStringToKey(s.Key), s)).ToList();
            var updatedValues = new List<KeyValuePair<TKey, TElement>>();
            foreach(var pair in existingValues)
            {
                TElement value;
                if(_items.TryGetValue(pair.Key, out value))
                {
                    // If necessary update the existing value.
                    switch(PropertyDef.ImplementationKind)
                    {
                        case ImplementationKind.ConfigurationCollection:
                        case ImplementationKind.ConfigurationDictionary:
                        case ImplementationKind.ConfigurationObject:
                            ((IConfigurationObject)value).Reload();
                            break;

                        default:
                            value = (TElement)PropertyDef.ConvertStringToValue(pair.Value.Value);
                            break;
                    }
                }
                else
                {
                    // If the value is a configuration object of some sort then create or reuse the existing value;
                    switch(PropertyDef.ImplementationKind)
                    {
                        case ImplementationKind.ConfigurationCollection:
                        case ImplementationKind.ConfigurationDictionary:
                            value = (TElement)Activator.CreateInstance(PropertyDef.ImplementationType, PropertyDef, ConfigurationRoot);
                            break;

                        case ImplementationKind.ConfigurationObject:
                            value = (TElement)Activator.CreateInstance(PropertyDef.ImplementationType, ConfigurationRoot);

                            break;

                        default:
                            value = (TElement)PropertyDef.ConvertStringToValue(pair.Value.Value);
                            break;
                    }

                    if(ReferenceEquals(value, null) && !PropertyDef.IsNullable)
                    {
                        throw new ConfigurationException(PropertyDef.Path, $"No value specified for configuration path: '{PropertyDef.Path}:{pair.Key}'.");
                    }

                    if(ReferenceEquals(value, null) && PropertyDef.IsNullable)
                    {
                        value = (TElement)PropertyDef.DefaultValue;
                    }
                }

                // Add/update the value in the updated values list.
                updatedValues.Add(new KeyValuePair<TKey, TElement>(pair.Key, value));
            }

            Lock.EnterWriteLock();
            try
            {
                Replace(updatedValues);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Add, updatedValues.Select(p => p.Value).ToList()));
        }

        /// <summary>
        ///     Removes the element with the specified key from the <see cref="IDictionary{T,T}" />.
        /// </summary>
        /// <param name="key"> The key of the element to remove. </param>
        /// <returns>
        ///     <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.
        ///     This method also returns <see langword="false" /> if <paramref name="key" /> was not found in the
        ///     original <see cref="IDictionary{T,T}" />.
        /// </returns>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        public virtual bool Remove(TKey key)
        {
            EnforceDisposed();
            EnforceReadOnly();

            NotifyCollectionChangedEventArgs? args = null;

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

                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed.Value);
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            if(ReferenceEquals(args, null))
            {
                return false;
            }

            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(args);
            return true;
        }

        ///// <summary>
        /////     Returns an enumerator that iterates through the collection.
        ///// </summary>
        ///// <returns> An enumerator that can be used to iterate through the collection. </returns>
        //public IEnumerator<KeyValuePair<TKey, TElement>> GetEnumerator()
        //{
        //    EnforceDisposed();
        /// <summary>
        ///     Removes the first occurrence of a specific object from the <see cref="ICollection{T}" />.
        /// </summary>
        /// <param name="item"> The object to remove from the <see cref="ICollection{T}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> was successfully removed from the
        ///     <see cref="ICollection{T}" />; otherwise, <see langword="false" />. This method also returns
        ///     <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="ICollection{T}" />.
        /// </returns>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        public bool Remove(KeyValuePair<TKey, TElement> item)
        {
            EnforceDisposed();
            EnforceReadOnly();

            NotifyCollectionChangedEventArgs? args = null;

            Lock.EnterWriteLock();
            try
            {
                if(_items.Remove(item.Key, out var removed))
                {
                    if(!_orderedItems.Remove(item))
                    {
                        Debug.Assert(false, "A matching item must always be found.");
                    }

                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item.Value);
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            if(ReferenceEquals(args, null))
            {
                return false;
            }

            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(args);
            return true;
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
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        public virtual bool Remove(TElement item)
        {
            EnforceDisposed();
            EnforceReadOnly();

            NotifyCollectionChangedEventArgs? args = null;

            Lock.EnterWriteLock();
            try
            {
                foreach(var element in _items.ToArray())
                {
                    if(Equals(element.Value, item))
                    {
                        if(!_items.Remove(element.Key, out var removedElement))
                        {
                            Debug.Assert(false, "We assume the element can be removed if it can be found.");
                        }

                        var n = 0;
                        foreach(var orderedElement in _orderedItems.ToArray())
                        {
                            if(Equals(orderedElement.Value, item))
                            {
                                _orderedItems.RemoveAt(n);
                                args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedElement, n);
                                break;
                            }
                            ++n;
                        }
                    }
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            if(ReferenceEquals(args, null))
            {
                return false;
            }

            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(args);
            return true;
        }

        /// <summary>
        ///     Saves this current values for each property back to the configuration sources.
        /// </summary>
        public void Save()
        {
            var values = _orderedItems.Select(p => p.Value);
        }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key"> The key whose value to get. </param>
        /// <param name="value">
        ///     When this method returns, the value associated with the specified key, if the key is found; otherwise,
        ///     the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the object that implements <see cref="IDictionary{T,T}" /> contains an
        ///     element with the specified key; otherwise, <see langword="false" />.
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
        /// <returns> An <see cref="IEnumerator" /> object that can be used to iterate through the collection. </returns>
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
        ///     Converts the string given to the key.
        /// </summary>
        /// <param name="key"> The key to convert, as a string. </param>
        /// <returns> Returns the key converted to the correct type. </returns>
        internal abstract TKey ConvertStringToKey(string key);

        /// <summary>
        ///     Converts the elements from the stream given into a stream of key-value pairs.
        /// </summary>
        /// <param name="elements"> The elements to convery. </param>
        /// <returns>
        ///     A stream of key-value pairs, initialized from the sequence given, with the key derived from each
        ///     elements position in the sequence.
        /// </returns>
        protected static IEnumerable<KeyValuePair<int, TElement>> GetIndexedElements(IEnumerable<TElement>? elements)
        {
            if(ReferenceEquals(elements, null))
            {
                yield break;
            }

            var n = 0;
            foreach(var element in elements)
            {
                yield return new KeyValuePair<int, TElement>(n++, element);
            }
        }

        /// <summary>
        ///     Converts the elements from the stream given into a stream of key-value pairs.
        /// </summary>
        /// <param name="elements"> The elements to convery. </param>
        /// <returns>
        ///     A stream of key-value pairs, initialized from the sequence given, with the key derived from each
        ///     elements property name.
        /// </returns>
        protected static IEnumerable<KeyValuePair<string, TElement>> GetKeyedElements(IEnumerable<TElement>? elements)
        {
            if(ReferenceEquals(elements, null))
            {
                yield break;
            }

            foreach(var element in elements)
            {
                yield return new KeyValuePair<string, TElement>(element.PropertyDef.PropertyName, element);
            }
        }

        /// <summary>
        ///     Determines whether this instance contains the object.
        /// </summary>
        /// <param name="item"> The object to locate in the <see cref="ICollection{T}" />. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> is found in the <see cref="ICollection{T}" />;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        protected bool ContainsValue(TElement item)
        {
            EnforceDisposed();

            Lock.EnterReadLock();
            try
            {
                foreach(var value in _orderedItems)
                    if(Equals(value.Value, item))
                        return true;

                return false;
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
        ///     Copies the elements of the <see cref="ICollection{T}" /> to an <see cref="Array" />, starting at a
        ///     particular <see cref="Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="Array" /> that is the destination of the elements copied from
        ///     <see cref="ICollection{T}" />. The <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex"> The zero-based index in <paramref name="array" /> at which copying begins. </param>
        /// <remarks> Assumes the caller already holds a read or write lock. </remarks>
        protected void InnerCopyTo(KeyValuePair<TKey, TElement>[] array, int arrayIndex)
        {
            _orderedItems.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     Called when the collection has changed.
        /// </summary>
        /// <param name="args"> The <see cref="NotifyCollectionChangedEventArgs" /> instance defining the change. </param>
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if(DisableEvents)
            {
                return;
            }

            if(IsDisposed)
            {
                return;
            }

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

            foreach(var callback in callbacks)
            {
                callback.DynamicInvoke(this, args);
            }
        }

        /// <summary>
        ///     Replaces the contents of the dictionary.
        /// </summary>
        /// <param name="list"> The new contents. </param>
        /// <remarks> Assumes that a write lock is held by the caller. </remarks>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        protected void Replace(IEnumerable<KeyValuePair<TKey, TElement>> list)
        {
            EnforceReadOnly();

            _orderedItems.Clear();
            _orderedItems.InsertRange(0, list);
            _items.Clear();
            foreach(var item in list)
            {
                _items.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        ///     Enforces the read-only property.
        /// </summary>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        private void EnforceReadOnly()
        {
            if(IsReadOnly)
            {
                throw new NotImplementedException("This collection is read-only.");
            }
        }

        /// <summary>
        ///     Removes all items from the <see cref="ICollection{T}" />.
        /// </summary>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
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

        /// <summary>
        ///     Sets the value by looking up the key.
        /// </summary>
        /// <param name="key"> The key identifying the value. </param>
        /// <param name="value"> The new value. </param>
        private void SetValueByKey(TKey key, TElement value)
        {
            NotifyCollectionChangedEventArgs? args = null;

            Lock.EnterUpgradeableReadLock();
            try
            {
                _items[key] = value;
                var n = 0;
                foreach(var element in _orderedItems)
                {
                    if(Equals(element.Value, value))
                    {
                        Lock.EnterWriteLock();
                        try
                        {
                            var existing = _orderedItems[n];
                            _orderedItems[n] = new KeyValuePair<TKey, TElement>(key, value);
                            _items.Remove(existing.Key);
                            _items.Add(key, value);
                            args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, existing, n);
                        }
                        finally
                        {
                            Lock.ExitWriteLock();
                        }
                        break;
                    }
                    ++n;
                }

                if(ReferenceEquals(args, null))
                {
                    _items.Add(key, value);
                    _orderedItems.Add(new KeyValuePair<TKey, TElement>(key, value));
                }
            }
            finally
            {
                Lock.ExitUpgradeableReadLock();
            }

            System.Diagnostics.Debug.Assert(!ReferenceEquals(args, null));

            if(args.Action == NotifyCollectionChangedAction.Add)
            {
                OnPropertyChanged(nameof(Count));
            }
            OnCollectionChanged(args);
        }

        /// <summary>
        ///     Sets the value by looking up the value.
        /// </summary>
        /// <param name="key"> The key identifying the value. </param>
        /// <param name="value"> The new value. </param>
        private void SetValueByValue(TKey key, TElement value)
        {
            NotifyCollectionChangedEventArgs? args = null;

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
                                args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, element.Value, n);
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
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value);
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

            System.Diagnostics.Debug.Assert(!ReferenceEquals(args, null));

            if(args.Action == NotifyCollectionChangedAction.Add)
            {
                OnPropertyChanged(nameof(Count));
            }
            OnCollectionChanged(args);
        }
    }
}