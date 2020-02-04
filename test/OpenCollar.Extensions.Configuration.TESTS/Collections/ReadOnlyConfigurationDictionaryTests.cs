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
using System.Collections.Generic;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS.Collections
{
    /// <summary>
    ///     Tests for the <see cref="Configuration.Collections.ReadOnlyConfigurationDictionary{T}" /> class.
    /// </summary>
    public sealed class ReadOnlyConfigurationDictionaryTests : IClassFixture<TestDataFixture>
    {
        private readonly TestDataFixture _propertyTestData;

        public ReadOnlyConfigurationDictionaryTests(TestDataFixture propertyDefFixture)
        {
            _propertyTestData = propertyDefFixture;
        }

        [Fact]
        public void AddAndRetrieveTests()
        {
            var testContext = _propertyTestData.GetContext();

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

            Assert.Equal(3, x.Count);

            Assert.Throws<NotImplementedException>(() => x.Add(new KeyValuePair<string, IChildElement>("a", a)));
            Assert.Throws<NotImplementedException>(() => x.Add("a", a));

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
            Assert.Throws<ObjectDisposedException>(() => x.Add("a", a));
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
            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c), Get("d", d));

            Assert.Equal(4, x.Count);

            Assert.Throws<NotImplementedException>(() => x.Clear());

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Clear());
        }

        [Fact]
        public void ContainsTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var d = testContext.GetChildElement("d");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b));

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
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

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
            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot);
            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.ContainsKey("a"));
        }

        [Fact]
        public void EnumeratorTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot);

            foreach(var item in x)
            {
                Assert.True(false);
            }

            x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

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
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

            Assert.Equal(a, x["a"].Value);
            Assert.Equal(b, x["b"].Value);
            Assert.Equal(c, x["c"].Value);
            Assert.Throws<ArgumentOutOfRangeException>(() => x["d"]);

            Assert.Throws<NotImplementedException>(() => x["a"].Value = a);

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

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

            Assert.False(x.IsDirty);

            x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c), Get("e", e));

            Assert.True(x.IsDirty);
        }

        [Fact]
        public void IsReadOnlyTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot);

            Assert.True(x.IsReadOnly);
        }

        [Fact]
        public void KeysTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Keys);
        }

        [Fact]
        public void ReadOnlyKeysTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => ((IReadOnlyDictionary<string, IChildElement>)x).Keys);
        }

        [Fact]
        public void ReadOnlyValuesTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => ((IReadOnlyDictionary<string, IChildElement>)x).Values);
        }

        [Fact]
        public void RemoveTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

            Assert.Throws<NotImplementedException>(() => x.Remove(b));

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
            var propertyDef = testContext.ReadOnlyChildConfigurationDictionaryPropertyDef;

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ReadOnlyConfigurationDictionary<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot, (IEnumerable<KeyValuePair<string, IChildElement>>?)null);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");

            x = new ReadOnlyConfigurationDictionary<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b));

            Assert.NotNull(x);
            Assert.Equal(2, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ReadOnlyConfigurationDictionary<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot, (IEnumerable<KeyValuePair<string, IChildElement>>)(new[] { Get("a", a), Get("b", b) }));

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

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

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

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot);

            foreach(var item in (System.Collections.IEnumerable)x)
            {
                Assert.True(false);
            }

            x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

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

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(null, testContext.ReadOnlyChildConfigurationDictionaryPropertyDef, testContext.Configuration.ConfigurationRoot, Get("a", a), Get("b", b), Get("c", c));

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Values);
        }

        private static KeyValuePair<string, IChildElement> Get(string key, IChildElement value)
        {
            return new KeyValuePair<string, IChildElement>(key, value);
        }
    }
}