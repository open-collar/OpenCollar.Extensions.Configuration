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
 * Copyright © 2019-2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using OpenCollar.Extensions.Configuration.Collections;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS.Collections
{
    /// <summary>
    ///     Tests for the <see cref="ConfigurationDictionary{T}" /> class.
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

            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var change = (NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) => { change = args.Action; };

            x.AddCopy("a", a);

            Assert.Single(x);
            Assert.Equal(NotifyCollectionChangedAction.Add, change);

            x.AddCopy("b", b);
            x.AddCopy("c", c);

            Assert.Equal(3, x.Count);

            var n = 0;
            foreach(var item in x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(b, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 2:
                        Assert.Equal(c, item.Value, ConfigurationObjectComparer.Instance);
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
                        Assert.Equal(a, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(c, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }

            x.AddCopy("b", b);

            Assert.Equal(3, x.Count);

            n = 0;
            foreach(var item in x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(c, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 2:
                        Assert.Equal(b, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }

            Assert.Throws<ArgumentException>(() => x.AddCopy("b", b));

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.AddCopy("a", a));
            Assert.Throws<ObjectDisposedException>(() => x.AddCopy(new KeyValuePair<string, IChildElement>("a", a)));
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
            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c), Get("d", d));

            Assert.Equal(4, x.Count);

            var change = (NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) => { change = args.Action; };

            x.Clear();

            Assert.Equal(0, x.Count);
            Assert.Equal(NotifyCollectionChangedAction.Reset, change);

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
            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var d = testContext.GetChildElement("d");

            x.AddCopy("a", a);
            x.AddCopy("b", b);

            Assert.True(x.Contains(a));
            Assert.True(x.Contains(b));
            Assert.False(x.Contains(c));
            Assert.False(x.Contains(null));
            Assert.True(((ICollection<KeyValuePair<string, IChildElement>>)x).Contains(new KeyValuePair<string, IChildElement>("a", a)));
            Assert.False(((ICollection<KeyValuePair<string, IChildElement>>)x).Contains(new KeyValuePair<string, IChildElement>("d", d)));

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Contains(a));
            Assert.Throws<ObjectDisposedException>(() =>
                ((ICollection<KeyValuePair<string, IChildElement>>)x).Contains(new KeyValuePair<string, IChildElement>("a", a)));
        }

        [Fact]
        public void CopyToTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            x.AddCopy("a", a);
            x.AddCopy("b", b);
            x.AddCopy("c", c);

            var array = new KeyValuePair<string, IChildElement>[3];

            x.CopyTo(array, 0);

            Assert.Equal(a, array[0].Value, ConfigurationObjectComparer.Instance);
            Assert.Equal(b, array[1].Value, ConfigurationObjectComparer.Instance);
            Assert.Equal(c, array[2].Value, ConfigurationObjectComparer.Instance);

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
            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot);
            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.ContainsKey("A"));
        }

        [Fact]
        public void EnumeratorTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            foreach(var item in x)
            {
                Assert.True(false);
            }

            x.AddCopy("a", a);
            x.AddCopy("b", b);
            x.AddCopy("c", c);

            var n = 0;
            foreach(var item in x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(b, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 2:
                        Assert.Equal(c, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }

            n = 0;
            foreach(KeyValuePair<string, IChildElement> item in (IEnumerable)x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(b, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 2:
                        Assert.Equal(c, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() =>
            {
                foreach(var y in x)
                {
                }

                ;
            });
        }

        [Fact]
        public void IndexerTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var d = testContext.GetChildElement("d");

            x.AddCopy("a", a);
            x.AddCopy("b", b);
            x.AddCopy("c", c);

            Assert.Equal(a, x["a"].Value, ConfigurationObjectComparer.Instance);
            Assert.Equal(b, x["b"].Value, ConfigurationObjectComparer.Instance);
            Assert.Equal(c, x["c"].Value, ConfigurationObjectComparer.Instance);
            Assert.Throws<ArgumentOutOfRangeException>(() => x["d"]);

            x["a"].Value = a;

            var z = (IDictionary<string, IChildElement>)x;
            Assert.Equal(a, z["a"], ConfigurationObjectComparer.Instance);
            Assert.Equal(b, z["b"], ConfigurationObjectComparer.Instance);
            Assert.Equal(c, z["c"], ConfigurationObjectComparer.Instance);
            Assert.Throws<ArgumentOutOfRangeException>(() => z["d"]);
            z["a"] = a;

            var change = (NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) => { change = args.Action; };

            x["c"].Value = d;

            Assert.Equal(NotifyCollectionChangedAction.Replace, change);

            var n = 0;
            foreach(var item in x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(b, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 2:
                        Assert.Equal(d, item.Value, ConfigurationObjectComparer.Instance);
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

            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

            Assert.False(x.IsDirty);

            x.AddCopy(Get("e", e));

            Assert.True(x.IsDirty);
        }

        [Fact]
        public void IsReadOnlyTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            Assert.False(x.IsReadOnly);
        }

        [Fact]
        public void KeysTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

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
            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");

            var callbackCount = 0;

            x.PropertyChanged += (source, args) => { ++callbackCount; };

            x.AddCopy(Get("a", a));

            Assert.Equal(1, callbackCount);

            x.AddCopy(Get("b", b));

            Assert.Equal(2, callbackCount);
        }

        [Fact]
        public void RemoveTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var d = testContext.GetChildElement("d");

            x.AddCopy("a", a);
            x.AddCopy("b", b);
            x.AddCopy("c", c);

            var change = (NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) => { change = args.Action; };

            Assert.True(x.Remove(b));
            Assert.Equal(NotifyCollectionChangedAction.Remove, change);

            change = (NotifyCollectionChangedAction)(-1);
            Assert.False(x.Remove(b));
            Assert.Equal((NotifyCollectionChangedAction)(-1), change);

            Assert.Equal(2, x.Count);

            var n = 0;
            foreach(var item in x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(c, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }

            x.AddCopy("b", b);
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
            var propertyDef = testContext.ChildConfigurationDictionaryPropertyDef;

            var x = new ConfigurationDictionary<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ConfigurationDictionary<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot,
                (IEnumerable<KeyValuePair<string, IChildElement>>)null);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");

            x = new ConfigurationDictionary<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b));

            Assert.NotNull(x);
            Assert.Equal(2, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ConfigurationDictionary<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot,
                (IEnumerable<KeyValuePair<string, IChildElement>>)(new[] { Get("a", a), Get("b", b) }));

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

            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

            Assert.True(x.TryGetValue("a", out var found));
            Assert.Equal(a, found, ConfigurationObjectComparer.Instance);

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

            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            foreach(var item in (IEnumerable)x)
            {
                Assert.True(false);
            }

            x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

            var n = 0;
            foreach(KeyValuePair<string, IChildElement> item in (IEnumerable)x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(b, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    case 2:
                        Assert.Equal(c, item.Value, ConfigurationObjectComparer.Instance);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() =>
            {
                foreach(var y in (IEnumerable)x)
                {
                }

                ;
            });
        }

        [Fact]
        public void ValuesTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ConfigurationDictionary<IChildElement>(null, testContext.ChildConfigurationDictionaryPropertyDef,
                testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

            var values = x.Values;

            Assert.NotNull(values);
            Assert.Equal(3, values.Count);

            var n = 0;
            foreach(var value in values)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, value, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(b, value, ConfigurationObjectComparer.Instance);
                        break;

                    case 2:
                        Assert.Equal(c, value, ConfigurationObjectComparer.Instance);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Values);
        }

        private static KeyValuePair<string, IChildElement> Get(string key, IChildElement value)
        {
            return new KeyValuePair<string, IChildElement>(key, value);
        }
    }
}