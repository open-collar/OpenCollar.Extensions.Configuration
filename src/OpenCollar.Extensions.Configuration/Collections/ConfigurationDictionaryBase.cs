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

namespace OpenCollar.Extensions.Configuration.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using Microsoft.Extensions.Configuration;

    /// <summary>
    ///     A base class provide the functionality used by both dictionaries and collections.
    /// </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    /// <typeparam name="TElement"> The type of the element. </typeparam>
    /// <seealso cref="IConfigurationObject" />
    /// <seealso cref="IDictionary{TKey, TElement}" />
    /// <seealso cref="INotifyCollectionChanged" />
    [DebuggerDisplay("ConfigurationDictionaryBase[{Count}] ({GetPath()})")]
    public abstract class ConfigurationDictionaryBase<TKey, TElement> : NotifyPropertyChanged, IEnumerable, IConfigurationObject, IValueChanged, IConfigurationChild
    {
        /// <summary>
        ///     A dictionary containing the elements of the collection against a key.
        /// </summary>
        private readonly Dictionary<TKey, Element<TKey, TElement>> _itemsByKey = new Dictionary<TKey, Element<TKey, TElement>>();

        /// <summary>
        ///     An ordered list of the elements in the collection.
        /// </summary>
        private readonly List<Element<TKey, TElement>> _orderedItems = new List<Element<TKey, TElement>>();

        /// <summary>
        ///     A value indicating whether events are raised for changes on the current thread. Any value greater than
        ///     zero indicates events are not to be raised.
        /// </summary>
        [ThreadStatic]
        private int _disableCollectionChangedEvents;

        /// <summary>
        ///     The object that is the parent of this one, or <see langword="null" /> if this is the root.
        /// </summary>
        private IConfigurationParent? _parent;

        /// <summary>
        ///     Suspends the read-only functionality when greater than zero. Thread-static.
        /// </summary>
        [ThreadStatic]
        private int _suspendReadOnly = 0;

        /// <summary>
        ///     Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationDictionaryBase{TKey, TElement}" /> class.
        /// </summary>
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        /// <param name="configurationRoot">
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </param>
        protected ConfigurationDictionaryBase(IConfigurationParent? parent, PropertyDef propertyDef, IConfigurationRoot configurationRoot)
        {
            _parent = parent;
            PropertyDef = propertyDef;
            ConfigurationRoot = configurationRoot;

            RegisterReloadToken();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationDictionaryBase{TKey, TElement}" /> class.
        /// </summary>
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        /// <param name="items"> The elements with which to initialize to the collection. </param>
        /// <param name="configurationRoot">
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </param>
        protected ConfigurationDictionaryBase(IConfigurationParent? parent, PropertyDef propertyDef, IConfigurationRoot configurationRoot, IEnumerable<KeyValuePair<TKey, TElement>>? items) : this(parent, propertyDef, configurationRoot)
        {
            PropertyDef = propertyDef;

            if(!ReferenceEquals(items, null))
            {
                foreach(var item in items)
                {
                    Element<TKey, TElement> element;
                    Interlocked.Increment(ref _suspendReadOnly);
                    try
                    {
                        element = new Element<TKey, TElement>(propertyDef, this, item.Key) { Value = item.Value };
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _suspendReadOnly);
                    }
                    _itemsByKey.Add(item.Key, element);
                    _orderedItems.Add(element);
                    element.Saved();
                }
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
                    return _itemsByKey.Count;
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
                    return _itemsByKey.Values.Any(i => i.IsDirty);
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
        public bool IsReadOnly
        {
            get
            {
                if(_suspendReadOnly > 0)
                {
                    return false;
                }

                return InnerIsReadOnly;
            }
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
        ///     Gets the number of elements contained in the <see cref="ICollection{T}" />.
        /// </summary>
        /// <value> The number of elements contained in the <see cref="ICollection{T}" />. </value>
        /// <remarks> Assumes that the caller already holds a read or write lock. </remarks>
        protected int InnerCount
        {
            get
            {
                return _itemsByKey.Count;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="ICollection{T}" /> is read-only.
        /// </summary>
        protected abstract bool InnerIsReadOnly
        {
            get;
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
        protected IReadOnlyList<Element<TKey, TElement>> OrderedItems
        {
            get
            {
                return _orderedItems.AsReadOnly();
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
        public Element<TKey, TElement> this[TKey key]
        {
            get
            {
                EnforceDisposed();

                if(_itemsByKey.TryGetValue(key, out var value))
                {
                    return value;
                }

                throw new ArgumentOutOfRangeException(nameof(key), $"'{nameof(key)}' did not identify a valid element.");
            }
        }

        /// <summary>
        ///     Adds a new value with the key specified, copying the properties and elements from the value give,
        ///     returning the new value.
        /// </summary>
        /// <param name="key"> The key identifying the value to add. </param>
        /// <param name="value"> The value to copy. </param>
        /// <returns> The newly added element. </returns>
        /// <remarks>
        ///     Used to add objects and collections that have been constructed externally using alternate implementations.
        /// </remarks>
        public TElement AddCopy(TKey key, TElement value)
        {
            TElement copy;

            switch(PropertyDef.ElementImplementation.ImplementationKind)
            {
                case ImplementationKind.ConfigurationCollection:
                    if(ReferenceEquals(value, null))
                    {
                        copy = default;
                    }
                    else
                    {
                        copy = (TElement)Activator.CreateInstance(PropertyDef.ElementImplementation.ImplementationType, this, PropertyDef, ConfigurationRoot, value);
                    }
                    break;

                case ImplementationKind.ConfigurationDictionary:
                    if(ReferenceEquals(value, null))
                    {
                        copy = default;
                    }
                    else
                    {
                        copy = (TElement)Activator.CreateInstance(PropertyDef.ElementImplementation.ImplementationType, this, PropertyDef, ConfigurationRoot, value);
                    }
                    break;

                case ImplementationKind.ConfigurationObject:
                    if(ReferenceEquals(value, null))
                    {
                        copy = default;
                    }
                    else
                    {
                        var clone = Activator.CreateInstance(PropertyDef.ElementImplementation.ImplementationType, ConfigurationRoot, this);
                        ((ConfigurationObjectBase<TElement>)clone).Clone(value);
                        copy = (TElement)clone;
                    }
                    break;

                default:
                    copy = value;
                    break;
            }

            Add(key, copy);

            return copy;
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
                return _itemsByKey.ContainsKey(key);
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
        public void Delete()
        {
            foreach(var element in _orderedItems)
            {
                element.DeleteValue(ConfigurationRoot);
            }
        }

        /// <summary>
        ///     Gets the path to this configuration object.
        /// </summary>
        /// <returns> A string containing the path to this configuration object. </returns>
        public string GetPath()
        {
            if(ReferenceEquals(_parent, null))
            {
                return string.Empty;
            }

            // The collection itself doesn't feature in the path, only the child element name and parent property name.
            return _parent.GetPath();
        }

        /// <summary>
        ///     Loads all of the properties from the configuration sources, overwriting any unsaved changes.
        /// </summary>
        public void Load()
        {
            var path = GetPath();

            // Iterate across all of the elements in the path and then delete those not in the dictionary, and then
            // insert those that have been added.
            var section = ConfigurationRoot.GetSection(path);
            var existingValues = section.GetChildren().Select(s => new KeyValuePair<TKey, IConfigurationSection>(ConvertStringToKey(s.Key), s)).ToList();
            var updatedValues = new List<Element<TKey, TElement>>();
            var newValues = new List<Element<TKey, TElement>>();

            Interlocked.Increment(ref _suspendReadOnly);
            try
            {
                foreach(var pair in existingValues)
                {
                    Element<TKey, TElement> value;
                    if(!_itemsByKey.TryGetValue(pair.Key, out value))
                    {
                        // If the value is a configuration object of some sort then create or reuse the existing value;
                        value = new Element<TKey, TElement>(PropertyDef, this, pair.Key);
                        newValues.Add(value);

                        value.ReadValue(ConfigurationRoot);

                        // Add/update the value in the updated values list.
                        value.Saved();
                    }
                    updatedValues.Add(value);
                }
            }
            finally
            {
                Interlocked.Decrement(ref _suspendReadOnly);
            }

            // TODO: How should we deal with values that weren;t added from the source but were added by the consumer at runtime?  Flags?
            var deletedValues = _orderedItems.ToArray().Except(updatedValues).ToList();

            if((deletedValues.Count > 0) || (newValues.Count > 0))
            {
                Interlocked.Increment(ref _suspendReadOnly);
                try
                {
                    Lock.EnterWriteLock();
                    try
                    {
                        Replace(updatedValues);
                    }
                    finally
                    {
                        Lock.ExitWriteLock();
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref _suspendReadOnly);
                }

                if(deletedValues.Count > 0)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, deletedValues.Select(p => p.Value).ToList()));
                }

                if(newValues.Count > 0)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newValues.Select(p => p.Value).ToList()));
                }
            }
        }

        /// <summary>
        ///     Called when a value has changed.
        /// </summary>
        /// <param name="oldValue"> The old value. </param>
        /// <param name="newValue"> The new value. </param>
        public void OnValueChanged(IValue oldValue, IValue newValue)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldValue, newValue));
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
                if(_itemsByKey.Remove(key))
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
                foreach(var element in _itemsByKey.ToArray())
                {
                    var z = element.Value.StringValue;
                    if(UniversalComparer.Equals(element.Value.Value, item))
                    {
                        if(!_itemsByKey.Remove(element.Key, out var removedElement))
                        {
                            Debug.Assert(false, "We assume the element can be removed if it can be found.");
                        }

                        var n = 0;
                        foreach(var orderedElement in _orderedItems.ToArray())
                        {
                            if(UniversalComparer.Equals(orderedElement.Value, item))
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
            foreach(var element in _orderedItems)
            {
                element.WriteValue(ConfigurationRoot);
            }
        }

        /// <summary>
        ///     Sets the parent of a configuration object.
        /// </summary>
        /// <param name="parent"> The new parent object. </param>
        public void SetParent(IConfigurationParent? parent)
        {
            _parent = parent;
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
                if(_itemsByKey.TryGetValue(key, out var item))
                {
                    value = item.Value;
                    return true;
                }
                value = default;
                return false;
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
        ///     Gets the elements given as a sequence of <see cref="KeyValuePair{T, TElement}" /> keyed in the index.
        /// </summary>
        /// <param name="elements"> The elements to convert. </param>
        /// <returns> The sequence of elements given, as <see cref="KeyValuePair{T, TElement}" /> objects. </returns>
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
        ///     Adds the specified item to the collection, using the key specified.
        /// </summary>
        /// <param name="key"> The key used to identify the item to add. Must be unique. </param>
        /// <param name="value"> The value to assign to the value. </param>
        /// <exception cref="ArgumentException"> An item with the same key has already been added. </exception>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        /// <exception cref="TypeMismatchException"> Expected object of different type. </exception>
        protected void Add(TKey key, TElement value)
        {
            EnforceDisposed();
            EnforceReadOnly();

            if(PropertyDef.ElementImplementation.ImplementationKind != ImplementationKind.Naive)
            {
                if(!ReferenceEquals(value, null))
                {
                    if(PropertyDef.ElementImplementation.ImplementationType != value.GetType())
                    {
                        throw new TypeMismatchException($"Expected object of type {PropertyDef.ElementImplementation.ImplementationType.FullName}.", GetPath());
                    }
                }
            }

            NotifyCollectionChangedEventArgs? args = null;

            var element = new Element<TKey, TElement>(PropertyDef, this, key) { Value = value };

            Lock.EnterUpgradeableReadLock();
            try
            {
                if(_itemsByKey.TryAdd(key, element))
                {
                    Lock.EnterWriteLock();
                    try
                    {
                        _orderedItems.Add(element);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value);
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

            throw new ArgumentException($"An item with the same key has already been added: {key}.", nameof(value));
        }

        /// <summary>
        ///     Adds a new value with the key specified, returning the new value.
        /// </summary>
        /// <param name="key"> The key identifying the value to add. </param>
        /// <returns> The newly added value. </returns>
        protected TElement AddNew(TKey key)
        {
            TElement value;

            switch(PropertyDef.ElementImplementation.ImplementationKind)
            {
                case ImplementationKind.ConfigurationCollection:
                case ImplementationKind.ConfigurationDictionary:
                    value = (TElement)Activator.CreateInstance(PropertyDef.ElementImplementation.ImplementationType, this, PropertyDef, ConfigurationRoot);
                    break;

                case ImplementationKind.ConfigurationObject:
                    value = (TElement)Activator.CreateInstance(PropertyDef.ElementImplementation.ImplementationType, ConfigurationRoot, this);
                    break;

                default:
                    value = (TElement)(Activator.CreateInstance(PropertyDef.ElementImplementation.ImplementationType));
                    break;
            }

            Add(key, value);

            return value;
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
                    if(UniversalComparer.Equals(value.Value, item))
                        return true;

                return false;
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Disables the firing of the <see cref="INotifyCollectionChanged.CollectionChanged" /> event on the
        ///     current thread.
        /// </summary>
        protected void DisableCollectionChangedEvents()
        {
            Interlocked.Increment(ref _disableCollectionChangedEvents);

            Debug.Assert(_disableCollectionChangedEvents >= 0);
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
        ///     Enables the firing of the <see cref="INotifyCollectionChanged.CollectionChanged" /> event on the current thread.
        /// </summary>
        protected void EnableCollectionChangedEvents()
        {
            Interlocked.Decrement(ref _disableCollectionChangedEvents);

            Debug.Assert(_disableCollectionChangedEvents >= 0);
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
        protected void InnerCopyTo(KeyValuePair<TKey, Element<TKey, TElement>>[] array, int arrayIndex)
        {
            _orderedItems.Select(e => new KeyValuePair<TKey, Element<TKey, TElement>>(e.Key, e)).ToArray().CopyTo(array, arrayIndex);
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
        protected void InnerCopyTo(Element<TKey, TElement>[] array, int arrayIndex)
        {
            _orderedItems.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     Called when the collection has changed.
        /// </summary>
        /// <param name="args"> The <see cref="NotifyCollectionChangedEventArgs" /> instance defining the change. </param>
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if(_disableCollectionChangedEvents > 0)
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
        ///     Reindexes the items after the specified index after removing an item.
        /// </summary>
        /// <param name="removedIndex"> Index of the removed item. </param>
        /// <param name="getNewKey"> A function that returns the new key for reindexed items. </param>
        protected void Reindex(int removedIndex, Func<int, Element<TKey, TElement>, TKey> getNewKey)
        {
            if(removedIndex >= Count)
            {
                // The last item was removed, we don't need to do anything.
                return;
            }

            DisableCollectionChangedEvents();
            try
            {
                var elements = _orderedItems.ToArray();
                for(var n = removedIndex; n < Count; ++n)
                {
                    _itemsByKey.Remove(elements[n].Key);
                    elements[n].Key = getNewKey(n, elements[n]);
                    _itemsByKey.Add(elements[n].Key, elements[n]);
                }
            }
            finally
            {
                EnableCollectionChangedEvents();
            }
        }

        /// <summary>
        ///     Replaces the contents of the dictionary.
        /// </summary>
        /// <param name="list"> The new contents. </param>
        /// <remarks> Assumes that a write lock is held by the caller. </remarks>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        protected void Replace(IEnumerable<Element<TKey, TElement>> list)
        {
            EnforceReadOnly();

            _orderedItems.Clear();
            _orderedItems.InsertRange(0, list);
            _itemsByKey.Clear();
            foreach(var item in list)
            {
                var child = item as IConfigurationChild;
                if(!ReferenceEquals(child, null))
                {
                    child.SetParent(this);
                }

                _itemsByKey.Add(item.Key, item);
            }
        }

        /// <summary>
        ///     Enforces the read-only property.
        /// </summary>
        /// <exception cref="NotImplementedException"> This collection is read-only. </exception>
        private void EnforceReadOnly()
        {
            if(_suspendReadOnly > 0)
            {
                return;
            }

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
                _itemsByKey.Clear();
                _orderedItems.Clear();
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            OnPropertyChanged(nameof(Count));
        }

        /// <summary>
        ///     Called when a section in the configuration root has changed.
        /// </summary>
        /// <param name="sectionObject"> An object containing the section that has changed. </param>
        private void OnSectionChanged(object sectionObject)
        {
            Load();

            RegisterReloadToken();
        }

        /// <summary>Registers a reload token with the <see cref"_configurationRoot"/>.</summary>
        private void RegisterReloadToken()
        {
            var token = ConfigurationRoot.GetReloadToken();
            token.RegisterChangeCallback(OnSectionChanged, ConfigurationRoot);
        }
    }
}