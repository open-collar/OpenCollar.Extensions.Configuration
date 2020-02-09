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

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A class used to represent a property on an interface and its location in the configuration model.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("PropertyValue[{PropertyName,nq}={StringValue}] ({GetPath()})")]
    internal sealed class PropertyValue<TValue> : ValueBase<ConfigurationObjectBase, TValue>, IPropertyValue, IConfigurationParent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyValue{TValue}" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property to represent. </param>
        /// <param name="parent"> The parent configuration object for which this object represents a property. </param>
        public PropertyValue(PropertyDef propertyDef, ConfigurationObjectBase parent) : base(propertyDef, parent)
        {
        }

        /// <summary>
        ///     Gets a value indicating whether this container is read-only.
        /// </summary>
        /// <value> <see langword="true" /> if this container is read-only; otherwise, <see langword="false" />. </value>
        public bool IsReadOnly
        {
            get
            {
                return _propertyDef.IsReadOnly;
            }
        }

        /// <summary>
        ///     Gets the name of the property represented by this object.
        /// </summary>
        /// <value> The name of the property represented by this object. </value>
        public string PropertyName
        {
            get
            {
                return _propertyDef.PropertyName;
            }
        }

        /// <summary>
        ///     Gets the implementation details of the value object.
        /// </summary>
        /// <value> The implementation details of the value object. </value>
        protected override Implementation ValueImplementation
        {
            get
            {
                return _propertyDef.Implementation;
            }
        }

        /// <summary>
        ///     Gets the path to this configuration object.
        /// </summary>
        /// <returns> A string containing the path to this configuration object. </returns>
        public override string GetPath()
        {
            return _propertyDef.GetPath(_parent);
        }
    }
}