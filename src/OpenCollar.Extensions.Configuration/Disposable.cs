using System;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>A base class for disposable objects.</summary>
    /// <seealso cref="System.IDisposable"/>
    public abstract class Disposable : IDisposable
    {
        /// <summary>The value assigned to <see cref="_isDisposed"/> if the class has not been disposed of.</summary>
        private const int NotDisposed = 0;

        /// <summary>The value assigned to <see cref="_isDisposed"/> if the class has been disposed of.</summary>
        private const int Disposed = 1;

        /// <summary>The a flag used to track, in a thread-safe way, whether the object has been disposed of.</summary>
        private readonly int _isDisposed = NotDisposed;

        /// <summary>Gets a value indicating whether this instance has been disposed of.</summary>
        /// <value><see langword="true"/> if this instance is has been disposed of; otherwise, <see langword="false"/>.</value>
        protected bool IsDisposed => _isDisposed == Disposed;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing">
        ///     <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged
        ///     resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>Throws an <see cref="ObjectDisposedException"/> if this object has been disposed of.</summary>
        /// <exception cref="ObjectDisposedException">This method cannot be used after the object has been disposed of.</exception>
        protected void EnforceDisposed()
        {
            if(_isDisposed == Disposed)
            {
                return;
            }

            throw new ObjectDisposedException("This method cannot be used after the object has been disposed of.");
        }
    }
}