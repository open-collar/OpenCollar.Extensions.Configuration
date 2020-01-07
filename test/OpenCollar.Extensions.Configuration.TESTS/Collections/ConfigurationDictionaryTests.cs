﻿/*
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
    ///     Tests for the <see cref="OpenCollar.Extensions.Configuration.Collections.ConfigurationDictionary{T}" /> class.
    /// </summary>
    public sealed class ConfigurationDictionaryTests
    {
        [Fact]
        public void AddAndRetrieveTests()
        {
            var x = new ConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            x.Add(a);

            Assert.Single(x);

            x.Add("b", b);
            x.Add(c);

            Assert.Equal(3, x.Count);

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

            Assert.True(x.Remove(b));

            Assert.Equal(2, x.Count);

            n = 0;
            foreach(var item in x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item.Value);
                        break;

                    case 1:
                        Assert.Equal(c, item.Value);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }

            x.Add("b", b);

            Assert.Equal(3, x.Count);

            n = 0;
            foreach(var item in x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item.Value);
                        break;

                    case 1:
                        Assert.Equal(c, item.Value);
                        break;

                    case 2:
                        Assert.Equal(b, item.Value);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }

            Assert.Throws<ArgumentException>(() => x.Add("z", b));
        }

        [Fact]
        public void ContainsTests()
        {
            var x = new ConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");
            var d = TestValues.GetChildElement("d");

            x.Add("a", a);
            x.Add("b", b);

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
            var x = new ConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            x.Add("a", a);
            x.Add("b", b);
            x.Add("c", c);

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
            var x = new ConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");

            foreach(var item in x)
            {
                Assert.True(false);
            }

            x.Add("a", a);
            x.Add("b", b);
            x.Add("c", c);

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
            var x = new ConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");
            var d = TestValues.GetChildElement("d");
            var e = TestValues.GetChildElement("e");

            x.Add("a", a);
            x.Add("b", b);
            x.Add("c", c);

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

            x["a"] = a;
            x["c"] = c;

            n = 0;
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
        public void IsReadOnlyTests()
        {
            var x = new ConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            Assert.False(x.IsReadOnly);
        }

        [Fact]
        public void RemoveTests()
        {
            var x = new ConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            var a = TestValues.GetChildElement("a");
            var b = TestValues.GetChildElement("b");
            var c = TestValues.GetChildElement("c");
            var d = TestValues.GetChildElement("d");

            x.Add("a", a);
            x.Add("b", b);
            x.Add("c", c);

            Assert.True(x.Remove(b));
            Assert.False(x.Remove(b));

            Assert.Equal(2, x.Count);

            var n = 0;
            foreach(var item in x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item.Value);
                        break;

                    case 1:
                        Assert.Equal(c, item.Value);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }
        }

        /// <summary>
        ///     Tests for the constructor.
        /// </summary>
        [Xunit.Fact]
        public void TestConstructor()
        {
            var x = new ConfigurationDictionary<IChildElement>(new PropertyDef("x", "x", typeof(ConfigurationCollection<IChildElement>), false));

            Assert.NotNull(x);
        }
    }
}