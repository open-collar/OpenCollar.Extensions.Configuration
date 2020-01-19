using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public class DisposableTests
    {
        [Fact]
        public void TestDispose()
        {
            var x = new DisposableMock();

            Assert.NotNull(x);
            x.Dispose();
            Assert.Equal(1, x.DisposeCalls);
            x.Dispose();
            Assert.Equal(1, x.DisposeCalls);
        }

        [Fact]
        public void TestEnforceDisposed()
        {
            var x = new DisposableMock();

            Assert.NotNull(x);
            x.EnforceDisposed();
            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.EnforceDisposed());
        }

        [Fact]
        public void TestIsDisposed()
        {
            var x = new DisposableMock();

            Assert.NotNull(x);
            Assert.False(x.IsDisposed);
            x.Dispose();
            Assert.True(x.IsDisposed);
        }
    }
}