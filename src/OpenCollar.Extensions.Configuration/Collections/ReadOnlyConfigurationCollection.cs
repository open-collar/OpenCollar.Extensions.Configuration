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
    internal sealed class ReadOnlyConfigurationCollection<TElement> : ConfigurationDictionaryBase<int, TElement>, IReadOnlyCollection<TElement>
        where TElement : IConfigurationObject
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyConfigurationCollection{TElement}" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        public ReadOnlyConfigurationCollection(PropertyDef propertyDef) : base(propertyDef)
        {
        }

        public override bool IsReadOnly { get; }

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() => throw new NotImplementedException();
    }
}