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

namespace OpenCollar.Extensions.Configuration
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender">The object that has changed.</param>
    /// <param name="e">The <see cref="ItemChangedEventArgs" /> instance containing the event data.</param>
    public delegate void ItemChangedEventHandler(object sender, ItemChangedEventArgs e);

    /// <summary>
    /// A public interface used by collections and objects that raise an event when a child item changes.
    /// </summary>
    public interface INotifyItemChanged
    {
        /// <summary>
        /// Occurs when an item in a property or collection belonging to this object or a child object is changed.
        /// </summary>
        event ItemChangedEventHandler ItemChanged;
    }

    /// <summary>
    /// The arguments supplied to handlers of the <see cref="INotifyItemChanged.ItemChanged"/> event.
    /// </summary>
    public class ItemChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemChangedEventArgs"/> class.
        /// </summary>
        /// <param name="propertyDefinition">The definition of the property that has changed.</param>
        public ItemChangedEventArgs(PropertyDef propertyDefinition)
        {
            PropertyDefinition = propertyDefinition;
        }

        /// <summary>
        /// Gets the PropertyDefinition
        /// </summary>
        public PropertyDef PropertyDefinition { get; }
    }
}
