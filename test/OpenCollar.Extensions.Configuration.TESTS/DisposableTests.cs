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

using System;

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