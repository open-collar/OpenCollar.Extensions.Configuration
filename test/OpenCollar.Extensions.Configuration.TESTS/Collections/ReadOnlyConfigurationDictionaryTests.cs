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
    ///     Tests for the <see cref="Configuration.Collections.ReadOnlyConfigurationDictionary{T}" /> class.
    /// </summary>
    public sealed class ReadOnlyConfigurationDictionaryTests
    {
        [Fact]
        public void AddAndRetrieveTests()
        {
            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            var x = new ReadOnlyConfigurationCollection<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationCollection<IChildElement>), false), a, b, c);

            Assert.Equal(3, x.Count);

            Assert.Throws<NotImplementedException>(() => x.Add(new KeyValuePair<int, IChildElement>(0, a)));
            Assert.Throws<NotImplementedException>(() => x.Add(a));

            var n = 0;
            foreach(var item in x)
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
        public void ContainsTests()
        {
            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");
            var d = TestValues.GetChildElement("d");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), a, b);

            Assert.True(x.Contains(a));
            Assert.True(x.Contains(b));
            Assert.False(x.Contains(c));
            Assert.False(x.Contains(null));
            Assert.True(((ICollection<KeyValuePair<string, IChildElement>>)x).Contains(new KeyValuePair<string, IChildElement>("a", a)));
            Assert.False(((ICollection<KeyValuePair<string, IChildElement>>)x).Contains(new KeyValuePair<string, IChildElement>("d", d)));
        }

        [Fact]
        public void CopyToTests()
        {
            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), a, b, c);

            var array = new KeyValuePair<string, IChildElement>[3];

            x.CopyTo(array, 0);

            Assert.Equal(a, array[0].Value);
            Assert.Equal(b, array[1].Value);
            Assert.Equal(c, array[2].Value);

            Assert.Throws<ArgumentException>(() => x.CopyTo(array, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => x.CopyTo(array, -1));

            array = new KeyValuePair<string, IChildElement>[2];

            Assert.Throws<ArgumentException>(() => x.CopyTo(array, 0));
        }

        [Fact]
        public void EnumeratorTests()
        {
            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false));

            foreach(var item in x)
            {
                Assert.True(false);
            }

            x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), a, b, c);

            var n = 0;
            foreach(var item in x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item.Value);
                        break;

                    case 1:
                        Assert.Equal(b, item.Value);
                        break;

                    case 2:
                        Assert.Equal(c, item.Value);
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
            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), a, b, c);

            Assert.Equal(a, x["a"]);
            Assert.Equal(b, x["b"]);
            Assert.Equal(c, x["c"]);

            Assert.Throws<NotImplementedException>(() => x["a"] = a);
        }

        [Fact]
        public void KeysTests()
        {
            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), a, b, c);

            var keys = x.Keys;

            Assert.Equal(3, keys.Count);

            var n = 0;
            foreach(var key in keys)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a.PropertyDef.PropertyName, key);
                        break;

                    case 1:
                        Assert.Equal(b.PropertyDef.PropertyName, key);
                        break;

                    case 2:
                        Assert.Equal(c.PropertyDef.PropertyName, key);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }
        }

        [Fact]
        public void ValuesTests()
        {
            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), a, b, c);

            var values = x.Values;

            Assert.Equal(3, values.Count);

            var n = 0;
            foreach(var value in values)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, value);
                        break;

                    case 1:
                        Assert.Equal(b, value);
                        break;

                    case 2:
                        Assert.Equal(c, value);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }
        }

        [Fact]
        public void IsReadOnlyTests()
        {
            var x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false));

            Assert.True(x.IsReadOnly);
        }

        [Fact]
        public void RemoveTests()
        {
            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), a, b, c);

            Assert.Throws<NotImplementedException>(() => x.Remove(b));
        }

        /// <summary>
        ///     Tests for the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            var x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false));

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);

            x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), (IEnumerable<IChildElement>)null);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");

            x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), a, b);

            Assert.NotNull(x);
            Assert.Equal(2, x.Count);

            x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), (IEnumerable<IChildElement>)(new[] { a, b }));

            Assert.NotNull(x);
            Assert.Equal(2, x.Count);
        }

        [Fact]
        public void ReadOnlyKeysTests()
        {
            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), a, b, c);

            var keys = ((IReadOnlyDictionary<string, IChildElement>)x).Keys;

            Assert.NotNull(keys);

            var n = 0;
            foreach(var key in keys)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a.PropertyDef.PropertyName, key);
                        break;

                    case 1:
                        Assert.Equal(b.PropertyDef.PropertyName, key);
                        break;

                    case 2:
                        Assert.Equal(c.PropertyDef.PropertyName, key);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }
        }

        [Fact]
        public void ReadOnlyValuesTests()
        {
            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ReadOnlyConfigurationDictionary<IChildElement>), false), a, b, c);

            var values = ((IReadOnlyDictionary<string, IChildElement>)x).Values;

            Assert.NotNull(values);

            var n = 0;
            foreach(var value in values)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, value);
                        break;

                    case 1:
                        Assert.Equal(b, value);
                        break;

                    case 2:
                        Assert.Equal(c, value);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }
        }
    }
}