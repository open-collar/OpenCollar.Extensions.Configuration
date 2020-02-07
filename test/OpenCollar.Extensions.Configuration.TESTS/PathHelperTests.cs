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

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class PathHelperTests
    {
        [Fact]
        public void TestGetPath()
        {
            Assert.Equal(string.Empty, PathHelper.GetPath());
            Assert.Equal(string.Empty, PathHelper.GetPath(string.Empty));
            Assert.Equal(string.Empty, PathHelper.GetPath(null));
            Assert.Equal(string.Empty, PathHelper.GetPath(new string[0]));
            Assert.Equal("A:B", PathHelper.GetPath("A", "B"));
            Assert.Equal("A:B", PathHelper.GetPath("A", string.Empty, "B"));
            Assert.Equal("A:B", PathHelper.GetPath("A", null, "B"));
        }
    }
}