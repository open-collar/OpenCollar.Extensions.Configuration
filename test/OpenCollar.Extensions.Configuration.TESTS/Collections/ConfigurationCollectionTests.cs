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

using System.Collections.Generic;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS.Collections
{
    /// <summary>
    ///     Tests for the <see cref="OpenCollar.Extensions.Configuration.Collections.ConfigurationCollection{T}" /> class.
    /// </summary>
    public sealed class ConfigurationCollectionTests
    {
        [Fact]
        public void AddAndRetrieveTests()
        {
            var x = new ConfigurationCollection<IChildElement>();

            var a = (new Moq.Mock<IChildElement>()).Object;
            var b = (new Moq.Mock<IChildElement>()).Object;
            var c = (new Moq.Mock<IChildElement>()).Object;

            x.Add(a);

            Assert.Single(x);

            x.Add(b);
            x.Add(c);

            Assert.Equal(3, x.Count);

            Assert.Equal(a, x[0]);
            Assert.Equal(b, x[1]);
            Assert.Equal(c, x[2]);

            Assert.True(x.Remove(b));

            Assert.Equal(2, x.Count);

            Assert.Equal(a, x[0]);
            Assert.Equal(c, x[1]);

            x.Add(b);

            Assert.Equal(3, x.Count);

            Assert.Equal(a, x[0]);
            Assert.Equal(c, x[1]);
            Assert.Equal(b, x[2]);
        }

        [Fact]
        public void EnumeratorTests()
        {
            var x = new ConfigurationCollection<IChildElement>();

            var a = (new Moq.Mock<IChildElement>()).Object;
            var b = (new Moq.Mock<IChildElement>()).Object;
            var c = (new Moq.Mock<IChildElement>()).Object;

            foreach(var item in x)
            {
                Assert.True(false);
            }

            x.Add(a);
            x.Add(b);
            x.Add(c);

            var n = 0;
            foreach(var item in (IEnumerable<IChildElement>)x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item);

                        break;
                }
            }
        }

        [Fact]
        public void IndexOfTests()
        {
            var x = new ConfigurationCollection<IChildElement>();

            var a = (new Moq.Mock<IChildElement>()).Object;
            var b = (new Moq.Mock<IChildElement>()).Object;
            var c = (new Moq.Mock<IChildElement>()).Object;

            Assert.True(x.IndexOf(a) < 0);
            Assert.True(x.IndexOf(b) < 0);
            Assert.True(x.IndexOf(c) < 0);

            x.Add(a);
            x.Add(b);

            Assert.Equal(0, x.IndexOf(a));
            Assert.Equal(1, x.IndexOf(b));
            Assert.True(x.IndexOf(c) < 0);

            x.Add(c);

            Assert.Equal(0, x.IndexOf(a));
            Assert.Equal(1, x.IndexOf(b));
            Assert.Equal(2, x.IndexOf(c));

            Assert.True(x.IndexOf(null) < 0);
        }

        /// <summary>
        ///     Tests for the constructor.
        /// </summary>
        [Xunit.Fact]
        public void TestConstructor()
        {
            var x = new ConfigurationCollection<IChildElement>();

            Assert.NotNull(x);
        }
    }
}