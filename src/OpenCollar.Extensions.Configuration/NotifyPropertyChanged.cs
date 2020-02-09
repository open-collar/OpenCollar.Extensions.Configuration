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

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A base class providing an implementation of the <see cref="INotifyPropertyChanged" /> interface (and the
    ///     <see cref="IDisposable" /> interface.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    /// <seealso cref="IDisposable" />
    public abstract class NotifyPropertyChanged : Disposable, INotifyPropertyChanged
    {
        /// <summary>
        ///     A value indicating whether property changed events are fired. Applies to the current thread only.
        /// </summary>
        [ThreadStatic]
        private bool suspendPropertyChangedEvents;

        /// <summary>
        ///     Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Gets or sets a value indicating whether property changed events are fired.
        /// </summary>
        /// <value> <see langword="true" /> if property changed events are suspended; otherwise, <see langword="false" />. </value>
        protected bool SuspendPropertyChangedEvents
        {
            get => suspendPropertyChangedEvents; set => suspendPropertyChangedEvents = value;
        }

        /// <summary>
        ///     Called when an underlying property has been changed.
        /// </summary>
        /// <param name="propertyName"> The name of the property that has changed. </param>
        /// <exception cref="AggregateException"> One or more change event handlers threw an exception. </exception>
        protected void OnPropertyChanged(string propertyName)
        {
            if(SuspendPropertyChangedEvents)
            {
                return;
            }

            if(IsDisposed)
            {
                return;
            }

            var handler = PropertyChanged;
            if(ReferenceEquals(handler, null))
            {
                // No-one's listening, do nothing more.
                return;
            }

            var callbacks = handler.GetInvocationList();

            System.Diagnostics.Debug.Assert(callbacks.Length > 0);

            var args = new PropertyChangedEventArgs(propertyName);

            var exceptions = new List<Exception>();

            foreach(var callback in callbacks)
            {
                try
                {
                    callback.DynamicInvoke(this, args);
                }
                catch(Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if(exceptions.Count > 0)
            {
                throw new AggregateException("One or more change event handlers threw an exception.", exceptions);
            }
        }

        /// <summary>
        ///     Called when a property is to be changed.
        /// </summary>
        /// <typeparam name="T"> The type of the property. </typeparam>
        /// <param name="field"> The field to which the value is to be assigned. </param>
        /// <param name="value"> The value to assign. </param>
        /// <param name="propertyName"> The name of the property that has changed. </param>
        /// <returns> <see langword="true" /> if the property has changed; otherwise, <see langword="false" /> </returns>
        /// <remarks> Raises the <see cref="PropertyChanged" /> event if the value has changed. </remarks>
        protected bool OnPropertyChanged<T>(string propertyName, ref T field, T value)
        {
            if(UniversalComparer.Equals(field, value))
            {
                return false;
            }

            field = value;

            OnPropertyChanged(propertyName);

            return true;
        }
    }
}