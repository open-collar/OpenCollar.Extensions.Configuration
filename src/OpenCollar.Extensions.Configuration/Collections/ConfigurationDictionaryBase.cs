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

namespace OpenCollar.Extensions.Configuration.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// A base class provide the functionality used by both dictionaries and collections.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <seealso cref="System.Collections.Generic.IDictionary{TKey, TElement}" />
    /// <seealso cref="System.Collections.Specialized.INotifyCollectionChanged" />
    public abstract class ConfigurationDictionaryBase<TKey, TElement> : IDictionary<TKey, TElement>, System.Collections.Specialized.INotifyCollectionChanged, INotifyItemChanged, IConfigurationObject where TElement : IConfigurationObject
    {

        /// <summary>A thread-safe dictionary containing the elements of the dictionary recorded against the keys supplied.</summary>
        private readonly System.Collections.Concurrent.ConcurrentDictionary<TKey, TElement> _items = new System.Collections.Concurrent.ConcurrentDictionary<TKey, TElement>();

        /// <summary>
        /// Gets or sets the <see cref="TElement" /> with the specified key.
        /// </summary>
        /// <value>The <see cref="TElement" /> to get or set.</value>
        /// <param name="key">The key identifying the element to get or set.</param>
        /// <returns>The element specified by <paramref name="key"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="key"/> did not identify a valid element.</exception>
        public TElement this[TKey key]
        {
            get
            {
                if (_items.TryGetValue(key, out var value))
                    return value;
                throw new ArgumentOutOfRangeException(nameof(key), "'key' did not identify a valid element.");
            }
            set
            {
                // TODO: add change detection and notification.
                _items.AddOrUpdate(key, k => value, (k, oldValue) => value);
            }
        }

        /// <summary>
        /// Gets an <see cref="System.Collections.Generic.ICollection{T}" /> containing the keys of the <see cref="System.Collections.Generic.IDictionary{T,T}" />.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                // TODO: Return the keys in the correct order
                return _items.Keys;
            }
        }
        /// <summary>
        /// Gets an <see cref="System.Collections.Generic.ICollection{T}" /> containing the values in the <see cref="System.Collections.Generic.IDictionary{T,T}" />.
        /// </summary>
        public ICollection<TElement> Values
        {
            get;
        }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="System.Collections.Generic.ICollection{T}" />.
        /// </summary>
        public int Count
        {
            get
            {
                return _items.Count;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this object has any properties with unsaved changes.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this object has any properties with unsaved changes; otherwise, <see langword="false" />.
        /// </value>
        public bool IsDirty
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="System.Collections.Generic.ICollection{T}" /> is read-only.
        /// </summary>
        public abstract bool IsReadOnly
        {
            get;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event ItemChangedEventHandler ItemChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Add(TKey key, TElement value) => throw new NotImplementedException();
        public void Add(KeyValuePair<TKey, TElement> item) => throw new NotImplementedException();
        public void Clear() => throw new NotImplementedException();
        public bool Contains(KeyValuePair<TKey, TElement> item) => throw new NotImplementedException();
        public bool ContainsKey(TKey key) => throw new NotImplementedException();
        public void CopyTo(KeyValuePair<TKey, TElement>[] array, int arrayIndex) => throw new NotImplementedException();
        public void Dispose() => throw new NotImplementedException();
        public IEnumerator<KeyValuePair<TKey, TElement>> GetEnumerator() => throw new NotImplementedException();
        public void Reload() => throw new NotImplementedException();
        public bool Remove(TKey key) => throw new NotImplementedException();
        public bool Remove(KeyValuePair<TKey, TElement> item) => throw new NotImplementedException();
        public void Save() => throw new NotImplementedException();
        public bool TryGetValue(TKey key, out TElement value) => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }
}
