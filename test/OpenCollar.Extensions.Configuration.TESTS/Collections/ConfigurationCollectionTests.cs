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
    ///     Tests for the <see cref="Configuration.Collections.ConfigurationCollection{T}" /> class.
    /// </summary>
    public sealed class ConfigurationCollectionTests
    {
        [Fact]
        public void AddAndRetrieveTests()
        {
            var x = new ConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Add(a));
            Assert.Throws<ObjectDisposedException>(() => x.Count);
            Assert.Throws<ObjectDisposedException>(() => x[0]);
        }

        [Fact]
        public void ContainsTests()
        {
            var x = new ConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            x.Add(a);
            x.Add(b);

            Assert.Contains(a, x);
            Assert.Contains(b, x);
            Assert.DoesNotContain(c, x);
            Assert.DoesNotContain(null, x);

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Contains(a));
        }

        [Fact]
        public void CopyToTests()
        {
            var x = new ConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.CopyTo(array, 0));
        }

        [Fact]
        public void EnumeratorTests()
        {
            var x = new ConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { foreach(var y in x) { }; });
        }

        [Fact]
        public void IndexerTests()
        {
            var x = new ConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x[0]);
        }

        [Fact]
        public void TryGetValueTests()
        {

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");
            var d = TestValues.GetChildElement("d");

            var x = new ConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationDictionary<IChildElement>), false), a, b, c);

            Assert.True(x.TryGetValue("a", out var found));
            Assert.Equal(a, found);

            Assert.False(x.TryGetValue("d", out found));
            Assert.Null(found);
        }

        [Fact]
        public void IndexOfTests()
        {
            var x = new ConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.IndexOf(a));
        }

        [Fact]
        public void InsertTests()
        {
            var x = new ConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");
            var d = TestValues.GetChildElement("d");
            var e = TestValues.GetChildElement("e");

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Insert(1, b));
        }

        [Fact]
        public void IsReadOnlyTests()
        {
            var x = new ConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            Assert.False(x.IsReadOnly);
        }

        [Fact]
        public void DisposeTests()
        {
            var x = new ConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));
            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.IndexOf(null));
        }


        [Fact]
        public void ClearTests()
        {
            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");
            var d = TestValues.GetChildElement("d");
            var x = new ConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false), a, b, c, d);

            Assert.Equal(4, x.Count);

            x.Clear();

            Assert.Equal(0, x.Count);

            foreach(var item in x)
            {
                Assert.True(false);
            }

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Clear());
        }

        [Fact]
        public void RemoveTests()
        {
            var x = new ConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");
            var d = TestValues.GetChildElement("d");

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.RemoveAt(0));
            Assert.Throws<ObjectDisposedException>(() => x.Remove(a));
        }

        /// <summary>
        ///     Tests for the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            var propertyDef = new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false);

            var x = new ConfigurationCollection<IChildElement>(propertyDef);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ConfigurationCollection<IChildElement>(propertyDef, (IEnumerable<IChildElement>)null);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");

            x = new ConfigurationCollection<IChildElement>(propertyDef, a, b);

            Assert.NotNull(x);
            Assert.Equal(2, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ConfigurationCollection<IChildElement>(propertyDef, (IEnumerable<IChildElement>)(new[] { a, b }));

            Assert.NotNull(x);
            Assert.Equal(2, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);
        }
    }
}