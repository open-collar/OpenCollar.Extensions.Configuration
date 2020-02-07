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
    public sealed class PathAttributeTests
    {
        [Fact]
        public void TestConstructor()
        {
            const string path = @"PATH";
            var x = new PathAttribute(PathIs.Root, path);

            Assert.NotNull(x);
            Assert.Equal(path, x.Path);
            Assert.Equal(PathIs.Root, x.Usage);

            Assert.Throws<ArgumentException>(() => { new PathAttribute(PathIs.Root, null); });
            Assert.Throws<ArgumentException>(() => { new PathAttribute(PathIs.Root, string.Empty); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new PathAttribute(PathIs.Unknown, path); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new PathAttribute((PathIs)666, path); });
        }
    }
}