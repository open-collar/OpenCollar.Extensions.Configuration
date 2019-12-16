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
    /// <summary>A collection of <see cref="IConfigurationObject">configuration objects</see> that notifies when an element is added or removed.</summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <seealso cref="OpenCollar.Extensions.Configuration.IConfigurationCollection{TElement}"/>
    internal sealed class ConfigurationCollection<TElement> : ConfigurationDictionaryBase<int, TElement>, IConfigurationCollection<TElement>
        where TElement : IConfigurationObject
    {
        public override bool IsReadOnly { get; }

        public void Add(TElement item) => throw new NotImplementedException();

        public bool Contains(TElement item) => throw new NotImplementedException();

        public void CopyTo(TElement[] array, int arrayIndex) => throw new NotImplementedException();

        public int IndexOf(TElement item) => throw new NotImplementedException();

        public void Insert(int index, TElement item) => throw new NotImplementedException();

        public bool Remove(TElement item) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() => throw new NotImplementedException();
    }
}