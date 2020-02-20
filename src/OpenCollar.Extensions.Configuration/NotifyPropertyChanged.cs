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
using System.Diagnostics;
using System.Threading;

using OpenCollar.Extensions.Configuration.Resources;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A base class providing an implementation of the <see cref="INotifyPropertyChanged" /> interface (and the
    ///     <see cref="IDisposable" /> interface.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/NotifyPropertyChanged/NotifyPropertyChanged.svg" />
    /// </remarks>
    /// <seealso cref="INotifyPropertyChanged" />
    /// <seealso cref="IDisposable" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class NotifyPropertyChanged : Disposable, INotifyPropertyChanged
    {
        /// <summary>
        ///     A value indicating whether events are raised for changes. Any value greater than zero indicates events
        ///     are not to be raised. Thread-static.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ThreadLocal<int> _disablePropertyChangedEvents = new ThreadLocal<int>();

        /// <summary>
        ///     Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

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
                _disablePropertyChangedEvents.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Disables the firing of the <see cref="INotifyPropertyChanged.PropertyChanged" /> event on the current thread.
        /// </summary>
        protected void DisablePropertyChangedEvents()
        {
            _disablePropertyChangedEvents.Value = _disablePropertyChangedEvents.Value + 1;

            Debug.Assert(_disablePropertyChangedEvents.Value > 0);
        }

        /// <summary>
        ///     Enables the firing of the <see cref="INotifyPropertyChanged.PropertyChanged" /> event on the current thread.
        /// </summary>
        protected void EnablePropertyChangedEvents()
        {
            _disablePropertyChangedEvents.Value = _disablePropertyChangedEvents.Value - 1;

            Debug.Assert(_disablePropertyChangedEvents.Value >= 0);
        }

        /// <summary>
        ///     Called when an underlying property has been changed.
        /// </summary>
        /// <param name="propertyName"> The name of the property that has changed. </param>
        /// <exception cref="AggregateException"> One or more change event handlers threw an exception. </exception>
        protected void OnPropertyChanged(string propertyName)
        {
            if(_disablePropertyChangedEvents.Value > 0)
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

            Debug.Assert(callbacks.Length > 0);

            var args = new PropertyChangedEventArgs(propertyName);

            var exceptions = new List<Exception>();

            foreach(var callback in callbacks)
            {
                try
                {
                    callback.DynamicInvoke(this, args);
                }
#pragma warning disable CA1031 // We don't know what might be thrown here.
                catch(Exception ex)
#pragma warning restore CA1031
                {
                    exceptions.Add(ex);
                }
            }

            if(exceptions.Count > 0)
            {
                throw new AggregateException(Exceptions.EventHandlerThrewException, exceptions);
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