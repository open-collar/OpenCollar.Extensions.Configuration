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
using System.Threading;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A base class for disposable objects.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public abstract class Disposable : IDisposable
    {
        /// <summary>
        ///     The value assigned to <see cref="_isDisposed" /> if the class has been disposed of.
        /// </summary>
        private const int Disposed = 1;

        /// <summary>
        ///     The value assigned to <see cref="_isDisposed" /> if the class has not been disposed of.
        /// </summary>
        private const int NotDisposed = 0;

        /// <summary>
        ///     The a flag used to track, in a thread-safe way, whether the object has been disposed of.
        /// </summary>
        private int _isDisposed = NotDisposed;

        /// <summary>
        ///     Gets a value indicating whether this instance has been disposed of.
        /// </summary>
        /// <value> <see langword="true" /> if this instance is has been disposed of; otherwise, <see langword="false" />. </value>
        protected bool IsDisposed => _isDisposed == Disposed;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if(Interlocked.CompareExchange(ref _isDisposed, Disposed, NotDisposed) == Disposed)
            {
                return;
            }

            Dispose(true);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to
        ///     release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        ///     Throws an <see cref="ObjectDisposedException" /> if this object has been disposed of.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        protected void EnforceDisposed()
        {
            if(_isDisposed != Disposed)
            {
                return;
            }

            throw new ObjectDisposedException("This method cannot be used after the object has been disposed of.");
        }
    }
}