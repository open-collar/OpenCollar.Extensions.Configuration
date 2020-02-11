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
 * Copyright © 2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

namespace OpenCollar.Extensions.Configuration.Collections
{
    /// <summary>
    ///     A class used to represent a property on an interface and its location in the configuration model.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Element[{Key,nq}={StringValue}] ({GetPath()})")]
    public sealed class Element<TKey, TValue> : ValueBase<ConfigurationDictionaryBase<TKey, TValue>, TValue>, System.IEquatable<Element<TKey, TValue>>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Element{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property to represent. </param>
        /// <param name="parent"> The parent configuration dictionary for which this object represents a property. </param>
        /// <param name="key"> The key that identifies this element in the collection. </param>
        internal Element(PropertyDef propertyDef, ConfigurationDictionaryBase<TKey, TValue> parent, TKey key) : base(propertyDef, parent)
        {
            Key = key;
        }

        ///// <summary>
        /////     Initializes a new instance of the <see cref="Element{TKey,TValue}" /> class.
        ///// </summary>
        ///// <param name="propertyDef"> The definition of the property to represent. </param>
        ///// <param name="parent"> The parent configuration dictionary for which this object represents a property. </param>
        ///// <param name="key"> The key that identifies this element in the collection. </param>
        //internal Element(PropertyDef propertyDef, ConfigurationDictionaryBase<TKey, TValue> parent, TKey key) : base(propertyDef, parent, default)
        //{
        //    Key = key;
        //}

        /// <summary>
        ///     Gets the key that uniquely identified this element in the dictionary or collection.
        /// </summary>
        /// <value> The key that uniquely identified this element in the dictionary or collection. </value>
        public TKey Key
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets the implementation details of the value object.
        /// </summary>
        /// <value> The implementation details of the value object. </value>
        protected override Implementation ValueImplementation
        {
            get
            {
                return _propertyDef.ElementImplementation;
            }
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other"> An object to compare with this object. </param>
        /// <returns>
        ///     <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(Element<TKey, TValue> other)
        {
            if(ReferenceEquals(other, null))
            {
                return false;
            }
            if(ReferenceEquals(other, this))
            {
                return true;
            }
            return Equals(other.Key, this.Key);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj"> The <see cref="System.Object" /> to compare with this instance. </param>
        /// <returns>
        ///     <see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if(ReferenceEquals(obj, null))
            {
                return false;
            }

            var other = obj as Element<TKey, TValue>;

            return Equals(other);
        }

        /// <summary>
        ///     Gets the path to this configuration object.
        /// </summary>
        /// <returns> A string containing the path to this configuration object. </returns>
        public override string GetPath()
        {
            return PathHelper.GetPath(_parent.GetPath(), Key.ToString());
        }
    }
}