using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    internal class DisposableMock : Disposable
    {
        private int _disposeCalls;

        public int DisposeCalls
        {
            get
            {
                return _disposeCalls;
            }
        }

        public new bool IsDisposed
        {
            get
            {
                return base.IsDisposed;
            }
        }

        public new void EnforceDisposed()
        {
            base.EnforceDisposed();
        }

        protected override void Dispose(bool disposing)
        {
            ++_disposeCalls;

            base.Dispose(disposing);
        }
    }
}