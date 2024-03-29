﻿/*
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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using OpenCollar.Extensions.Configuration.Resources;

namespace OpenCollar.Extensions.Configuration.Collections
{
    /// <summary>
    ///     A base class provide the functionality used by both dictionaries and collections.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the key.
    /// </typeparam>
    /// <typeparam name="TElement">
    ///     The type of the element.
    /// </typeparam>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/Collections/ConfigurationDictionaryBase/ConfigurationDictionaryBase.svg" />
    /// </remarks>
    /// <seealso cref="IConfigurationObject" />
    /// <seealso cref="IDictionary{TKey,TValue}" />
    /// <seealso cref="INotifyCollectionChanged" />
    [DebuggerDisplay("ConfigurationDictionaryBase<{typeof(TElement).Name,nq}>[{Count}] ({CalculatePath()})")]
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class ConfigurationDictionaryBase<TKey, TElement> : NotifyPropertyChanged, IConfigurationObject, IValueChanged, IConfigurationChild,
    IConfigurationParent
    {
        /// <summary>
        ///     A value indicating whether events are raised for changes on the current thread. Any value greater than
        ///     zero indicates events are not to be raised.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ThreadLocal<int> _disableCollectionChangedEvents = new ThreadLocal<int>();

        /// <summary>
        ///     Suspends the read-only functionality when greater than zero. Thread-static.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ThreadLocal<int> _disableReadOnly = new ThreadLocal<int>();

        /// <summary>
        ///     A dictionary containing the elements of the collection against a key.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<TKey, Element<TKey, TElement>> _itemsByKey = new Dictionary<TKey, Element<TKey, TElement>>();

        /// <summary>
        ///     An ordered list of the elements in the collection.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<Element<TKey, TElement>> _orderedItems = new List<Element<TKey, TElement>>();

        /// <summary>
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </summary>
        private readonly ConfigurationObjectSettings _settings;

        /// <summary>
        ///     The object that is the parent of this one, or <see langword="null" /> if this is the root.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IConfigurationParent? _parent;

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
        /// <param name="propertyDef">
        ///     The definition of the property defined by this object.
        /// </param>
        /// <param name="configurationRoot">
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </param>
        /// <param name="settings">
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </param>
        protected ConfigurationDictionaryBase(IConfigurationParent? parent, IPropertyDef propertyDef, IConfigurationRoot configurationRoot, ConfigurationObjectSettings settings)
        {
            _parent = parent;
            PropertyDef = propertyDef;
            ConfigurationRoot = configurationRoot;
            _settings = settings;

            RegisterReloadToken();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationDictionaryBase{TKey, TElement}" /> class.
        /// </summary>
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        /// <param name="propertyDef">
        ///     The definition of the property defined by this object.
        /// </param>
        /// <param name="items">
        ///     The elements with which to initialize to the collection.
        /// </param>
        /// <param name="configurationRoot">
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </param>
        /// <param name="settings">
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </param>
        protected ConfigurationDictionaryBase(IConfigurationParent? parent, IPropertyDef propertyDef, IConfigurationRoot configurationRoot,
        IEnumerable<KeyValuePair<TKey, TElement>>? items, ConfigurationObjectSettings settings) : this(parent, propertyDef, configurationRoot, settings)
        {
            PropertyDef = propertyDef;

            if(!ReferenceEquals(items, null))
            {
                foreach(var item in items)
                {
                    Element<TKey, TElement> element;
                    _disableReadOnly.Value = _disableReadOnly.Value + 1;
                    try
                    {
                        element = new Element<TKey, TElement>(propertyDef, this, item.Key)
                        {
                            Value = item.Value
                        };
                    }
                    finally
                    {
                        _disableReadOnly.Value = _disableReadOnly.Value - 1;
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
        /// <value>
        ///     The number of elements contained in the <see cref="ICollection{T}" />.
        /// </value>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        public int Count
        {
            get
            {
                CheckNotDisposed();

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
                CheckNotDisposed();

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
                if(_disableReadOnly.Value > 0)
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
                CheckNotDisposed();

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
        /// <value>
        ///     The definition of this property object.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPropertyDef PropertyDef
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
                CheckNotDisposed();

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
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IConfigurationRoot ConfigurationRoot
        {
            get;
        }

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="ICollection{T}" />.
        /// </summary>
        /// <value>
        ///     The number of elements contained in the <see cref="ICollection{T}" />.
        /// </value>
        /// <remarks>
        ///     Assumes that the caller already holds a read or write lock.
        /// </remarks>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected int InnerCount => _itemsByKey.Count;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="ICollection{T}" /> is read-only.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected abstract bool InnerIsReadOnly
        {
            get;
        }

        /// <summary>
        ///     Gets the lock object used to control concurrent access to the collection.
        /// </summary>
        /// <value>
        ///     The lock object used to control concurrent access to the collection.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        ///     Gets the items in the dictionary as an ordered, read-only list.
        /// </summary>
        /// <value>
        ///     The items in the dictionary as an ordered, read-only list.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected IReadOnlyList<Element<TKey, TElement>> OrderedItems => _orderedItems.AsReadOnly();

        /// <summary>
        ///     Gets or sets the item with the specified key.
        /// </summary>
        /// <value>
        ///     The item to get or set.
        /// </value>
        /// <param name="key">
        ///     The key identifying the element to get or set.
        /// </param>
        /// <returns>
        ///     The element specified by <paramref name="key" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="key" /> did not identify a valid element.
        /// </exception>
        /// <exception cref="NotImplementedException">
        ///     This collection is read-only.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        public Element<TKey, TElement> this[TKey key]
        {
            get
            {
                CheckNotDisposed();

                if(_itemsByKey.TryGetValue(key, out var value))
                {
                    return value;
                }

                throw new ArgumentOutOfRangeException(nameof(key), key, string.Format(CultureInfo.CurrentCulture, Exceptions.KeyNotFound, nameof(key), key));
            }
        }

        /// <summary>
        ///     Adds a new value with the key specified, copying the properties and elements from the value give,
        ///     returning the new value.
        /// </summary>
        /// <param name="key">
        ///     The key identifying the value to add.
        /// </param>
        /// <param name="value">
        ///     The value to copy.
        /// </param>
        /// <returns>
        ///     The newly added element.
        /// </returns>
        /// <remarks>
        ///     Used to add objects and collections that have been constructed externally using alternate implementations.
        /// </remarks>
        public TElement AddCopy(TKey key, TElement value)
        {
            Debug.Assert(PropertyDef.ElementImplementation != null);

            var copy = PropertyDef.CopyValue(PropertyDef.ElementImplementation, value, this, ConfigurationRoot);

            Add(key, copy);

            return copy;
        }

        /// <summary>
        ///     Gets the path to this configuration object.
        /// </summary>
        /// <returns>
        ///     A string containing the path to this configuration object.
        /// </returns>
        public string CalculatePath()
        {
            if(ReferenceEquals(_parent, null))
            {
                return string.Empty;
            }

            // The collection itself doesn't feature in the path, only the child element name and parent property name.
            return _parent.CalculatePath();
        }

        /// <summary>
        ///     Removes all items from the <see cref="ICollection{T}" />.
        /// </summary>
        /// <exception cref="NotImplementedException">
        ///     This collection is read-only.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        public void Clear()
        {
            CheckNotDisposed();
            EnforceReadOnly();

            InternalClear();

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///     Determines whether the <see cref="IDictionary{T,T}" /> contains an element with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key to locate in the <see cref="IDictionary{T,T}" />.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the <see cref="IDictionary{T,T}" /> contains an element with the key;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        public bool ContainsKey(TKey key)
        {
            CheckNotDisposed();

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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Delete()
        {
            if(!PropertyDef.Persistence.HasFlag(ConfigurationPersistenceActions.SaveOnly))
            {
                return;
            }

            foreach(var element in _orderedItems)
            {
                element.DeleteValue(ConfigurationRoot);
            }
        }

        /// <summary>
        ///     Loads all of the properties from the configuration sources, overwriting any unsaved changes.
        /// </summary>
        public void Load()
        {
            Load(false);
        }

        /// <summary>
        ///     Called when a value has changed.
        /// </summary>
        /// <param name="oldValue">
        ///     The old value.
        /// </param>
        /// <param name="newValue">
        ///     The new value.
        /// </param>
        public void OnValueChanged(IValue oldValue, IValue newValue)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldValue, newValue));
        }

        /// <summary>
        ///     Removes the element with the specified key from the <see cref="IDictionary{T,T}" />.
        /// </summary>
        /// <param name="key">
        ///     The key of the element to remove.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.
        ///     This method also returns <see langword="false" /> if <paramref name="key" /> was not found in the
        ///     original <see cref="IDictionary{T,T}" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        ///     This collection is read-only.
        /// </exception>
        public virtual bool Remove(TKey key)
        {
            CheckNotDisposed();
            EnforceReadOnly();

            NotifyCollectionChangedEventArgs? args = null;

            Lock.EnterWriteLock();
            try
            {
                if(_itemsByKey.Remove(key))
                {
                    var removed = _orderedItems.First(i => i.Key!.Equals(key));
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
        /// <param name="item">
        ///     The object to remove from the <see cref="ICollection{T}" />.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> was successfully removed from the
        ///     <see cref="ICollection{T}" />; otherwise, <see langword="false" />. This method also returns
        ///     <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="ICollection{T}" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        ///     This collection is read-only.
        /// </exception>
        public virtual bool Remove(TElement item)
        {
            CheckNotDisposed();
            EnforceReadOnly();

            var args = RemoveInner(item);

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
            if(!PropertyDef.Persistence.HasFlag(ConfigurationPersistenceActions.SaveOnly))
            {
                return;
            }

            foreach(var element in _orderedItems)
            {
                element.WriteValue(ConfigurationRoot);
            }
        }

        /// <summary>
        ///     Sets the parent of a configuration object.
        /// </summary>
        /// <param name="parent">
        ///     The new parent object.
        /// </param>
        public void SetParent(IConfigurationParent? parent)
        {
            _parent = parent;
        }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key whose value to get.
        /// </param>
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
            CheckNotDisposed();

            Lock.EnterReadLock();
            try
            {
                if(_itemsByKey.TryGetValue(key, out var item))
                {
                    value = item.Value;
                    return true;
                }

                value = default!;
                return false;
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Converts the string given to the key.
        /// </summary>
        /// <param name="key">
        ///     The key to convert, as a string.
        /// </param>
        /// <returns>
        ///     Returns the key converted to the correct type.
        /// </returns>
        internal abstract TKey ConvertStringToKey(string key);

        /// <summary>
        ///     Loads all of the properties from the configuration sources, overwriting any unsaved changes.
        /// </summary>
        /// <param name="initializing">
        ///     If set to <see langword="true" /> the element changed events are not fired.
        /// </param>
        internal void Load(bool initializing)
        {
            if(!PropertyDef.Persistence.HasFlag(ConfigurationPersistenceActions.LoadOnly))
            {
                return;
            }

            var path = CalculatePath();

            // Iterate across all of the elements in the path and then delete those not in the dictionary, and then
            // insert those that have been added.
            var section = ConfigurationRoot.GetSection(path);
            var existingValues = section.GetChildren().Select(s => new KeyValuePair<TKey, IConfigurationSection>(ConvertStringToKey(s.Key), s)).ToList();
            var updatedValues = new List<Element<TKey, TElement>>();
            var newValues = new List<Element<TKey, TElement>>();

            InnerLoad(initializing, existingValues, newValues, updatedValues);

            // TODO: How should we deal with values that weren't added from the source but were added by the consumer at runtime?  Flags?
            var deletedValues = _orderedItems.ToArray().Except(updatedValues).ToList();

            if((deletedValues.Count > 0) || (newValues.Count > 0))
            {
                _disableReadOnly.Value = _disableReadOnly.Value + 1;
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
                    _disableReadOnly.Value = _disableReadOnly.Value - 1;
                }

                if(deletedValues.Count > 0)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                    deletedValues.Select(p => p.Value).ToList()));
                }

                if(newValues.Count > 0)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newValues.Select(p => p.Value).ToList()));
                }
            }
        }

        /// <summary>
        ///     Gets the elements given as a sequence of <see cref="KeyValuePair{T, TElement}" /> keyed in the index.
        /// </summary>
        /// <param name="elements">
        ///     The elements to convert.
        /// </param>
        /// <returns>
        ///     The sequence of elements given, as <see cref="KeyValuePair{T, TElement}" /> objects.
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
        ///     Adds the specified item to the collection, using the key specified.
        /// </summary>
        /// <param name="key">
        ///     The key used to identify the item to add. Must be unique.
        /// </param>
        /// <param name="value">
        ///     The value to assign to the value.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     An item with the same key has already been added.
        /// </exception>
        /// <exception cref="NotImplementedException">
        ///     This collection is read-only.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        /// <exception cref="TypeMismatchException">
        ///     Expected object of different type.
        /// </exception>
        protected void Add(TKey key, TElement value)
        {
            CheckNotDisposed();
            EnforceReadOnly();

            Debug.Assert(PropertyDef.ElementImplementation != null);

            if(PropertyDef.ElementImplementation.ImplementationKind != ImplementationKind.Naive)
            {
                if(!ReferenceEquals(value, null))
                {
                    if(PropertyDef.ElementImplementation.ImplementationType != value.GetType())
                    {
                        Debug.Assert(PropertyDef.ElementImplementation.ImplementationType != null);
                        throw new TypeMismatchException(null, $"Expected object of type {PropertyDef.ElementImplementation.ImplementationType.FullName}.",
                        CalculatePath());
                    }
                }
            }

            NotifyCollectionChangedEventArgs? args = null;

            var element = new Element<TKey, TElement>(PropertyDef, this, key)
            {
                Value = value
            };

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

            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Exceptions.DuplicateKey, key), nameof(value));
        }

        /// <summary>
        ///     Adds a new value with the key specified, returning the new value.
        /// </summary>
        /// <param name="key">
        ///     The key identifying the value to add.
        /// </param>
        /// <returns>
        ///     The newly added value.
        /// </returns>
        protected TElement AddNew(TKey key)
        {
            TElement value;

            Debug.Assert(PropertyDef.ElementImplementation != null);

            switch(PropertyDef.ElementImplementation.ImplementationKind)
            {
                case ImplementationKind.ConfigurationCollection:
                case ImplementationKind.ConfigurationDictionary:
                    value = (TElement)Activator.CreateInstance(PropertyDef.ElementImplementation.ImplementationType, this, PropertyDef, ConfigurationRoot, PropertyDef.Settings);
                    break;

                case ImplementationKind.ConfigurationObject:
                    var validators = ServiceCollectionExtensions.GetValidators(PropertyDef.ElementImplementation.Type);
                    value = (TElement)Activator.CreateInstance(PropertyDef.ElementImplementation.ImplementationType, PropertyDef, ConfigurationRoot, this, PropertyDef.Settings, validators);
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
        /// <param name="item">
        ///     The object to locate in the <see cref="ICollection{T}" />.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> is found in the <see cref="ICollection{T}" />;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        protected bool ContainsValue(TElement item)
        {
            CheckNotDisposed();

            Lock.EnterReadLock();
            try
            {
                foreach(var value in _orderedItems)
                {
                    if(UniversalComparer.Equals(value.Value, item))
                    {
                        return true;
                    }
                }

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
            _disableCollectionChangedEvents.Value = _disableCollectionChangedEvents.Value + 1;

            Debug.Assert(_disableCollectionChangedEvents.Value > 0);
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
                _disableCollectionChangedEvents.Dispose();
                _disableReadOnly.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Enables the firing of the <see cref="INotifyCollectionChanged.CollectionChanged" /> event on the current thread.
        /// </summary>
        protected void EnableCollectionChangedEvents()
        {
            _disableCollectionChangedEvents.Value = _disableCollectionChangedEvents.Value - 1;

            Debug.Assert(_disableCollectionChangedEvents.Value >= 0);
        }

        /// <summary>
        ///     Copies the elements of the <see cref="ICollection{T}" /> to an <see cref="Array" />, starting at a
        ///     particular <see cref="Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="Array" /> that is the destination of the elements copied from
        ///     <see cref="ICollection{T}" />. The <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based index in <paramref name="array" /> at which copying begins.
        /// </param>
        /// <remarks>
        ///     Assumes the caller already holds a read or write lock.
        /// </remarks>
        protected void InnerCopyTo(KeyValuePair<TKey, Element<TKey, TElement>>[] array, int arrayIndex)
        {
            _orderedItems.Select(e => new KeyValuePair<TKey, Element<TKey, TElement>>(e.Key, e)).ToArray().CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     Called when the collection has changed.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="NotifyCollectionChangedEventArgs" /> instance defining the change.
        /// </param>
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if(_disableCollectionChangedEvents.Value > 0)
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
        ///     Re-indexes the items after the specified index after removing an item.
        /// </summary>
        /// <param name="removedIndex">
        ///     Index of the removed item.
        /// </param>
        /// <param name="getNewKey">
        ///     A function that returns the new key for re-indexed items.
        /// </param>
        protected void ReIndex(int removedIndex, Func<int, Element<TKey, TElement>, TKey> getNewKey)
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
        /// <param name="list">
        ///     The new contents.
        /// </param>
        /// <remarks>
        ///     Assumes that a write lock is held by the caller.
        /// </remarks>
        /// <exception cref="NotImplementedException">
        ///     This collection is read-only.
        /// </exception>
        protected void Replace(IEnumerable<Element<TKey, TElement>> list)
        {
            EnforceReadOnly();

            var newItems = list.ToArray();

            _orderedItems.Clear();
            _orderedItems.InsertRange(0, newItems);
            _itemsByKey.Clear();
            foreach(var item in newItems)
            {
                var child = item as IConfigurationChild;
                child?.SetParent(this);

                _itemsByKey.Add(item.Key, item);
            }
        }

        /// <summary>
        ///     Enforces the read-only property.
        /// </summary>
        /// <exception cref="NotImplementedException">
        ///     This collection is read-only.
        /// </exception>
        private void EnforceReadOnly()
        {
            if(_disableReadOnly.Value > 0)
            {
                return;
            }

            if(IsReadOnly)
            {
                throw new NotImplementedException(Exceptions.CollectionIsReadOnly);
            }
        }

        /// <summary>
        ///     Loads all of the properties from the configuration sources, overwriting any unsaved changes.
        /// </summary>
        /// <param name="initializing">
        ///     If set to <see langword="true" /> the element changed events are not fired.
        /// </param>
        /// <param name="existingValues">
        ///     A list of the existing values.
        /// </param>
        /// <param name="newValues">
        ///     A list to which to add the new values.
        /// </param>
        /// <param name="updatedValues">
        ///     A list to which to add the updated values.
        /// </param>
        private void InnerLoad(bool initializing, List<KeyValuePair<TKey, IConfigurationSection>> existingValues, List<Element<TKey, TElement>> newValues, List<Element<TKey, TElement>> updatedValues)
        {
            // Assumes validation has already been performed.

            if(initializing)
            {
                _disableReadOnly.Value = _disableReadOnly.Value + 1; // TODO: What about config changes after load
            }

            try
            {
                foreach(var pair in existingValues)
                {
                    if(!_itemsByKey.TryGetValue(pair.Key, out var value))
                    {
                        // If the value is a configuration object of some sort then create or reuse the existing value.
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
                if(initializing)
                {
                    _disableReadOnly.Value = _disableReadOnly.Value - 1;
                }
            }
        }

        /// <summary>
        ///     Removes all items from the <see cref="ICollection{T}" />.
        /// </summary>
        /// <exception cref="NotImplementedException">
        ///     This collection is read-only.
        /// </exception>
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
        /// <param name="sectionObject">
        ///     An object containing the section that has changed.
        /// </param>
        private void OnSectionChanged(object sectionObject)
        {
            _disableReadOnly.Value = _disableReadOnly.Value + 1;
            try
            {
                Load(false);
            }
            finally
            {
                _disableReadOnly.Value = _disableReadOnly.Value - 1;
            }

            RegisterReloadToken();
        }

        /// <summary>
        ///     Registers a reload token with the <see cref="ConfigurationRoot" />.
        /// </summary>
        private void RegisterReloadToken()
        {
            var token = ConfigurationRoot.GetReloadToken();
            token.RegisterChangeCallback(OnSectionChanged, ConfigurationRoot);
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the <see cref="ICollection{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The object to remove from the <see cref="ICollection{T}" />.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> was successfully removed from the
        ///     <see cref="ICollection{T}" />; otherwise, <see langword="false" />. This method also returns
        ///     <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="ICollection{T}" />.
        /// </returns>
        private NotifyCollectionChangedEventArgs RemoveInner(TElement item)
        {
            // Assumes all validation has been performed. Does not fire any events.

            Lock.EnterWriteLock();
            try
            {
                foreach(var element in _itemsByKey.ToArray())
                {
                    if(UniversalComparer.Equals(element.Value.Value, item))
                    {
                        _itemsByKey.Remove(element.Key, out var removedElement);

                        var n = 0;
                        foreach(var orderedElement in _orderedItems.ToArray())
                        {
                            if(UniversalComparer.Equals(orderedElement.Value, item))
                            {
                                _orderedItems.RemoveAt(n);
                                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedElement, n);
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

            return null;
        }
    }
}