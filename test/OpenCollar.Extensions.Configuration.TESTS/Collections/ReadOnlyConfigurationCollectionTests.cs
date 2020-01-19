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
    ///     Tests for the <see cref="Configuration.Collections.ReadOnlyConfigurationCollection{T}" /> class.
    /// </summary>
    public sealed class ReadOnlyConfigurationCollectionTests : IClassFixture<TestDataFixture>
    {
        private readonly TestDataFixture _propertyTestData;

        public ReadOnlyConfigurationCollectionTests(TestDataFixture propertyDefFixture)
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

            var x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef, a, b, c);

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

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Add(a));
            Assert.Throws<ObjectDisposedException>(() => x.Count);
            Assert.Throws<ObjectDisposedException>(() => x[0]);
        }

        [Fact]
        public void ClearTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var d = testContext.GetChildElement("d");
            var x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef, a, b, c, d);

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

            var x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef, a, b);

            Assert.True(x.Contains(a));
            Assert.True(x.Contains(b));
            Assert.False(x.Contains(c));
            Assert.False(x.Contains(null));

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Contains(a));
        }

        [Fact]
        public void CopyToTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef, a, b, c);

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
        public void DisposeTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationCollection<IChildElement>(testContext.PropertyDef);
            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.IndexOf(null));
        }

        [Fact]
        public void EnumeratorTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef);

            foreach(var item in x)
            {
                Assert.True(false);
            }

            x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef, a, b, c);

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
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef, a, b, c);

            Assert.Equal(a, x[0]);
            Assert.Equal(b, x[1]);
            Assert.Equal(c, x[2]);

            Assert.Throws<NotImplementedException>(() => x[0] = c);

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x[0]);
        }

        [Fact]
        public void IndexOfTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef);

            Assert.True(x.IndexOf(a) < 0);
            Assert.True(x.IndexOf(b) < 0);
            Assert.True(x.IndexOf(c) < 0);

            x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef, a, b);

            Assert.Equal(0, x.IndexOf(a));
            Assert.Equal(1, x.IndexOf(b));
            Assert.True(x.IndexOf(c) < 0);

            x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef, a, b, c);

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
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");

            var x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef, a);

            Assert.Throws<NotImplementedException>(() => x.Insert(1, b));

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Insert(1, b));
        }

        [Fact]
        public void IsDirtyTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var e = testContext.GetChildElement("e");

            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, a, b, c);

            Assert.False(x.IsDirty);

            x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, a, b, c, e);

            Assert.True(x.IsDirty);
        }

        [Fact]
        public void IsReadOnlyTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef);

            Assert.True(x.IsReadOnly);
        }

        [Fact]
        public void RemoveTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");

            var x = new ReadOnlyConfigurationCollection<IChildElement>(testContext.PropertyDef, a);

            Assert.Throws<NotImplementedException>(() => x.Remove(b));
            Assert.Throws<NotImplementedException>(() => x.RemoveAt(0));
            Assert.Throws<NotImplementedException>(() => x.Remove(2));
        }

        /// <summary>
        ///     Tests for the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            var testContext = _propertyTestData.GetContext();
            var propertyDef = testContext.PropertyDef;

            var x = new ReadOnlyConfigurationCollection<IChildElement>(propertyDef);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ReadOnlyConfigurationCollection<IChildElement>(propertyDef, (IEnumerable<IChildElement>)null);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");

            x = new ReadOnlyConfigurationCollection<IChildElement>(propertyDef, a, b);

            Assert.NotNull(x);
            Assert.Equal(2, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ReadOnlyConfigurationCollection<IChildElement>(propertyDef, (IEnumerable<IChildElement>)(new[] { a, b }));

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

            var x = new ConfigurationDictionary<IChildElement>(testContext.PropertyDef, a, b, c);

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

            var x = new ReadOnlyConfigurationDictionary<IChildElement>(testContext.PropertyDef);

            foreach(var item in (System.Collections.IEnumerable)x)
            {
                Assert.True(false);
            }

            x = new ReadOnlyConfigurationDictionary<IChildElement>(testContext.PropertyDef, a, b, c);

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
    }
}