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

using System;
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
        public void ContainsTests()
        {
            var x = new ConfigurationCollection<IChildElement>();

            var a = (new Moq.Mock<IChildElement>()).Object;
            var b = (new Moq.Mock<IChildElement>()).Object;
            var c = (new Moq.Mock<IChildElement>()).Object;

            x.Add(a);
            x.Add(b);

            Assert.True(x.Contains(a));
            Assert.True(x.Contains(b));
            Assert.False(x.Contains(c));
            Assert.False(x.Contains(null));
        }

        [Fact]
        public void CopyToTests()
        {
            var x = new ConfigurationCollection<IChildElement>();

            var a = (new Moq.Mock<IChildElement>()).Object;
            var b = (new Moq.Mock<IChildElement>()).Object;
            var c = (new Moq.Mock<IChildElement>()).Object;

            x.Add(a);
            x.Add(b);
            x.Add(c);

            var array = new IChildElement[3];

            x.CopyTo(array, 0);

            Assert.Equal(a, array[0]);
            Assert.Equal(b, array[1]);
            Assert.Equal(c, array[2]);

            Assert.Throws<ArgumentOutOfRangeException>(() => x.CopyTo(array, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => x.CopyTo(array, -1));

            array = new IChildElement[2];

            Assert.Throws<ArgumentOutOfRangeException>(() => x.CopyTo(array, 0));
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

                    case 1:
                        Assert.Equal(b, item);
                        break;

                    case 2:
                        Assert.Equal(c, item);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }
        }

        [Fact]
        public void IndexerTests()
        {
            var x = new ConfigurationCollection<IChildElement>();

            var a = (new Moq.Mock<IChildElement>()).Object;
            var b = (new Moq.Mock<IChildElement>()).Object;
            var c = (new Moq.Mock<IChildElement>()).Object;

            x.Add(a);
            x.Add(b);
            x.Add(c);

            Assert.Equal(a, x[0]);
            Assert.Equal(b, x[1]);
            Assert.Equal(c, x[2]);

            x[0] = c;
            x[2] = a;

            Assert.Equal(c, x[0]);
            Assert.Equal(b, x[1]);
            Assert.Equal(a, x[2]);
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

        [Fact]
        public void InsertTests()
        {
            var x = new ConfigurationCollection<IChildElement>();

            var a = (new Moq.Mock<IChildElement>()).Object;
            var b = (new Moq.Mock<IChildElement>()).Object;
            var c = (new Moq.Mock<IChildElement>()).Object;
            var d = (new Moq.Mock<IChildElement>()).Object;
            var e = (new Moq.Mock<IChildElement>()).Object;

            x.Add(a);
            x.Add(c);

            x.Insert(1, b);

            Assert.Equal(3, x.Count);

            Assert.Equal(a, x[0]);
            Assert.Equal(b, x[1]);
            Assert.Equal(c, x[2]);

            x.Insert(x.Count, d);

            Assert.Equal(4, x.Count);
            Assert.Equal(a, x[0]);
            Assert.Equal(b, x[1]);
            Assert.Equal(c, x[2]);
            Assert.Equal(d, x[3]);

            x.Insert(0, e);

            Assert.Equal(5, x.Count);
            Assert.Equal(e, x[0]);
            Assert.Equal(a, x[1]);
            Assert.Equal(b, x[2]);
            Assert.Equal(c, x[3]);
            Assert.Equal(d, x[4]);

            Assert.Throws<ArgumentOutOfRangeException>(() => { x.Insert(-1, a); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { x.Insert(x.Count + 1, a); });
        }

        [Fact]
        public void IsReadOnlyTests()
        {
            var x = new ConfigurationCollection<IChildElement>();

            Assert.False(x.IsReadOnly);
        }

        [Fact]
        public void RemoveTests()
        {
            var x = new ConfigurationCollection<IChildElement>();

            var a = (new Moq.Mock<IChildElement>()).Object;
            var b = (new Moq.Mock<IChildElement>()).Object;
            var c = (new Moq.Mock<IChildElement>()).Object;
            var d = (new Moq.Mock<IChildElement>()).Object;

            x.Add(a);
            x.Add(b);
            x.Add(c);

            Assert.True(x.Remove(b));
            Assert.False(x.Remove(b));

            Assert.Equal(2, x.Count);
            Assert.Equal(a, x[0]);
            Assert.Equal(c, x[1]);

            Assert.True(x.Remove(0));

            Assert.Equal(1, x.Count);
            Assert.Equal(c, x[0]);

            x.RemoveAt(0);

            Assert.Equal(0, x.Count);

            x.Add(a);
            x.Add(b);
            x.Add(c);

            x.RemoveAt(2);

            Assert.Equal(2, x.Count);
            Assert.Equal(a, x[0]);
            Assert.Equal(b, x[1]);

            Assert.False(x.Remove(d));
            Assert.False(x.Remove(2));
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