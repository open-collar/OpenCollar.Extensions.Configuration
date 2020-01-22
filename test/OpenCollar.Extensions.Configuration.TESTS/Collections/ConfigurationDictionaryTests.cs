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
    ///     Tests for the <see cref="Configuration.Collections.ConfigurationDictionary{T}" /> class.
    /// </summary>
    public sealed class ConfigurationDictionaryTests : IClassFixture<TestDataFixture>
    {
        private readonly TestDataFixture _propertyTestData;

        public ConfigurationDictionaryTests(TestDataFixture propertyDefFixture)
        {
            _propertyTestData = propertyDefFixture;
        }

        [Fact]
        public void AddAndRetrieveTests()
        {
            var testContext = _propertyTestData.GetContext();

            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var change = (System.Collections.Specialized.NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) =>
            {
                change = args.Action;
            };

            x.Add(a);

            Assert.Single(x);
            Assert.Equal(System.Collections.Specialized.NotifyCollectionChangedAction.Add, change);

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

            Assert.Throws<ArgumentException>(() => x.Add("b", b));

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Add("a", a));
            Assert.Throws<ObjectDisposedException>(() => x.Add(a));
            Assert.Throws<ObjectDisposedException>(() => x.Count);
        }

        [Fact]
        public void ClearTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var d = testContext.GetChildElement("d");
            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot, a, b, c, d);

            Assert.Equal(4, x.Count);

            var change = (System.Collections.Specialized.NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) =>
            {
                change = args.Action;
            };

            x.Clear();

            Assert.Equal(0, x.Count);
            Assert.Equal(System.Collections.Specialized.NotifyCollectionChangedAction.Reset, change);

            foreach(var item in x)
            {
                Assert.True(false);
            }

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Clear());
        }

        [Fact]
        public void ContainsTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var d = testContext.GetChildElement("d");

            x.Add("a", a);
            x.Add("b", b);

            Assert.True(x.Contains(a));
            Assert.True(x.Contains(b));
            Assert.False(x.Contains(c));
            Assert.False(x.Contains(null));
            Assert.True(((ICollection<KeyValuePair<string, IChildElement>>)x).Contains(new KeyValuePair<string, IChildElement>("a", a)));
            Assert.False(((ICollection<KeyValuePair<string, IChildElement>>)x).Contains(new KeyValuePair<string, IChildElement>("d", d)));

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Contains(a));
            Assert.Throws<ObjectDisposedException>(() => ((ICollection<KeyValuePair<string, IChildElement>>)x).Contains(new KeyValuePair<string, IChildElement>("a", a)));
        }

        [Fact]
        public void CopyToTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.CopyTo(array, 0));
        }

        [Fact]
        public void DisposeTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot);
            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.ContainsKey("A"));
        }

        [Fact]
        public void EnumeratorTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { foreach(var y in x) { }; });
        }

        [Fact]
        public void IndexerTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            x.Add("a", a);
            x.Add("b", b);
            x.Add("c", c);

            Assert.Equal(a, x["a"]);
            Assert.Equal(b, x["b"]);
            Assert.Equal(c, x["c"]);
            Assert.Throws<ArgumentOutOfRangeException>(() => x["d"]);

            x["a"] = a;

            var change = (System.Collections.Specialized.NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) =>
            {
                change = args.Action;
            };

            x["c"] = c;

            Assert.Equal(System.Collections.Specialized.NotifyCollectionChangedAction.Replace, change);

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x["a"]);
        }

        [Fact]
        public void IsDirtyTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var e = testContext.GetChildElement("e");

            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot, a, b, c);

            Assert.False(x.IsDirty);

            x.Add(e);

            Assert.True(x.IsDirty);
        }

        [Fact]
        public void IsReadOnlyTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot);

            Assert.False(x.IsReadOnly);
        }

        [Fact]
        public void KeysTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot, a, b, c);

            var keys = x.Keys;

            Assert.NotNull(keys);
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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Keys);
        }

        [Fact]
        public void PropertyChangedTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");

            var callbackCount = 0;

            x.PropertyChanged += (source, args) =>
            {
                ++callbackCount;
            };

            x.Add(a);

            Assert.Equal(1, callbackCount);

            x.Add(b);

            Assert.Equal(2, callbackCount);
        }

        [Fact]
        public void RemoveTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var d = testContext.GetChildElement("d");

            x.Add("a", a);
            x.Add("b", b);
            x.Add("c", c);

            var change = (System.Collections.Specialized.NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) =>
            {
                change = args.Action;
            };

            Assert.True(x.Remove(b));
            Assert.Equal(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, change);

            change = (System.Collections.Specialized.NotifyCollectionChangedAction)(-1);
            Assert.False(x.Remove(b));
            Assert.Equal((System.Collections.Specialized.NotifyCollectionChangedAction)(-1), change);

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

            x.Add("b", b);
            Assert.True(x.Remove(new KeyValuePair<string, IChildElement>("b", b)));
            Assert.False(x.Remove(new KeyValuePair<string, IChildElement>("b", b)));

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Remove(a));
        }

        /// <summary>
        ///     Tests for the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            var testContext = _propertyTestData.GetContext();
            var propertyDef = testContext.PropertyDef;

            var x = new ConfigurationDictionary<IChildElement>(propertyDef, testContext.Configuration.ConfigurationRoot);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ConfigurationDictionary<IChildElement>(propertyDef, testContext.Configuration.ConfigurationRoot, (IEnumerable<IChildElement>)null);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");

            x = new ConfigurationDictionary<IChildElement>(propertyDef, testContext.Configuration.ConfigurationRoot, a, b);

            Assert.NotNull(x);
            Assert.Equal(2, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ConfigurationDictionary<IChildElement>(propertyDef, testContext.Configuration.ConfigurationRoot, (IEnumerable<IChildElement>)(new[] { a, b }));

            Assert.NotNull(x);
            Assert.Equal(2, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);
        }

        [Fact]
        public void TryGetValueTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var d = testContext.GetChildElement("d");

            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot, a, b, c);

            Assert.True(x.TryGetValue("a", out var found));
            Assert.Equal(a, found);

            Assert.False(x.TryGetValue("d", out found));
            Assert.Null(found);
        }

        [Fact]
        public void UntypedEnumeratorTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot);

            foreach(var item in (System.Collections.IEnumerable)x)
            {
                Assert.True(false);
            }

            x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot, a, b, c);

            var n = 0;
            foreach(KeyValuePair<string, IChildElement> item in (System.Collections.IEnumerable)x)
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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { foreach(var y in (System.Collections.IEnumerable)x) { }; });
        }

        [Fact]
        public void ValuesTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, testContext.Configuration.ConfigurationRoot, a, b, c);

            var values = x.Values;

            Assert.NotNull(values);
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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Values);
        }
    }
}