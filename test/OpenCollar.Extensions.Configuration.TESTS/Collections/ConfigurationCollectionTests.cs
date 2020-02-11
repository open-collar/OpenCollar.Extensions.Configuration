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
    /// <summary>Tests for the <see cref="Configuration.Collections.ConfigurationCollection{T}"/> class.</summary>
    public sealed class ConfigurationCollectionTests : IClassFixture<TestDataFixture>
    {
        public ConfigurationCollectionTests(TestDataFixture propertyDefFixture)
        {
            _propertyTestData = propertyDefFixture;
        }

        private readonly TestDataFixture _propertyTestData;

        [Fact]
        public void AddAndRetrieveTests()
        {
            var testContext = _propertyTestData.GetContext();

            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var change = (NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) => { change = args.Action; };

            x.AddCopy(a);

            Assert.Single(x);
            Assert.Equal(NotifyCollectionChangedAction.Add, change);

            x.AddCopy(b);
            x.AddCopy(c);

            Assert.Equal(3, x.Count);

            Assert.Equal(a, x[0], ConfigurationObjectComparer.Instance);
            Assert.Equal(b, x[1], ConfigurationObjectComparer.Instance);
            Assert.Equal(c, x[2], ConfigurationObjectComparer.Instance);

            Assert.True(x.Remove(b));

            Assert.Equal(2, x.Count);

            Assert.Equal(a, x[0], ConfigurationObjectComparer.Instance);
            Assert.Equal(c, x[1], ConfigurationObjectComparer.Instance);

            x.AddCopy(b);

            Assert.Equal(3, x.Count);

            Assert.Equal(a, x[0], ConfigurationObjectComparer.Instance);
            Assert.Equal(c, x[1], ConfigurationObjectComparer.Instance);
            Assert.Equal(b, x[2], ConfigurationObjectComparer.Instance);

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.AddCopy(a));
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
            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot, a, b, c, d);

            Assert.Equal(4, x.Count);

            var change = (NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) => { change = args.Action; };

            x.Clear();

            Assert.Equal(NotifyCollectionChangedAction.Reset, change);

            Assert.Equal(0, x.Count);

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
            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            x.AddCopy(a);
            x.AddCopy(b);

            Assert.Contains(a, x, ConfigurationObjectComparer.Instance);
            Assert.Contains(b, x, ConfigurationObjectComparer.Instance);
            Assert.DoesNotContain(c, x, ConfigurationObjectComparer.Instance);
            Assert.DoesNotContain(null, x, ConfigurationObjectComparer.Instance);

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.Contains(a));
        }

        [Fact]
        public void CopyToTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            x.AddCopy(a);
            x.AddCopy(b);
            x.AddCopy(c);

            var array = new IChildElement[3];

            x.CopyTo(array, 0);

            Assert.Equal(a, array[0], ConfigurationObjectComparer.Instance);
            Assert.Equal(b, array[1], ConfigurationObjectComparer.Instance);
            Assert.Equal(c, array[2], ConfigurationObjectComparer.Instance);

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
            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);
            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.IndexOf(null));
        }

        [Fact]
        public void EnumeratorTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            foreach(var item in x)
            {
                Assert.True(false);
            }

            x.AddCopy(a);
            x.AddCopy(b);
            x.AddCopy(c);

            var n = 0;
            foreach(var item in (IEnumerable<IChildElement>)x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(b, item, ConfigurationObjectComparer.Instance);
                        break;

                    case 2:
                        Assert.Equal(c, item, ConfigurationObjectComparer.Instance);
                        break;

                    default:
                        Assert.True(false);
                        break;
                }
            }

            n = 0;

            foreach(IChildElement item in (IEnumerable)x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(b, item, ConfigurationObjectComparer.Instance);
                        break;

                    case 2:
                        Assert.Equal(c, item, ConfigurationObjectComparer.Instance);
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
            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            x.AddCopy(a);
            x.AddCopy(b);
            x.AddCopy(c);

            Assert.Equal(a, x[0], ConfigurationObjectComparer.Instance);
            Assert.Equal(b, x[1], ConfigurationObjectComparer.Instance);
            Assert.Equal(c, x[2], ConfigurationObjectComparer.Instance);

            var change = (NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) => { change = args.Action; };

            x[0] = c;

            Assert.Equal(NotifyCollectionChangedAction.Replace, change);

            x[2] = a;

            Assert.Equal(c, x[0], ConfigurationObjectComparer.Instance);
            Assert.Equal(b, x[1], ConfigurationObjectComparer.Instance);
            Assert.Equal(a, x[2], ConfigurationObjectComparer.Instance);

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x[0]);
        }

        [Fact]
        public void IndexOfTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            Assert.True(x.IndexOf(a) < 0);
            Assert.True(x.IndexOf(b) < 0);
            Assert.True(x.IndexOf(c) < 0);

            x.AddCopy(a);
            x.AddCopy(b);

            Assert.Equal(0, x.IndexOf(a));
            Assert.Equal(1, x.IndexOf(b));
            Assert.True(x.IndexOf(c) < 0);

            x.AddCopy(c);

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
            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var d = testContext.GetChildElement("d");
            var e = testContext.GetChildElement("e");

            x.AddCopy(a);
            x.AddCopy(c);

            var changes = new List<NotifyCollectionChangedAction>();
            x.CollectionChanged += (sender, args) => { changes.Add(args.Action); };

            x.InsertCopy(1, b);

            Assert.Equal(2, changes.Count);
            Assert.Equal(NotifyCollectionChangedAction.Add, changes[0]);
            Assert.Equal(NotifyCollectionChangedAction.Move, changes[1]);

            Assert.Equal(3, x.Count);

            Assert.Equal(a, x[0], ConfigurationObjectComparer.Instance);
            Assert.Equal(b, x[1], ConfigurationObjectComparer.Instance);
            Assert.Equal(c, x[2], ConfigurationObjectComparer.Instance);

            x.InsertCopy(x.Count, d);

            Assert.Equal(4, x.Count);
            Assert.Equal(a, x[0], ConfigurationObjectComparer.Instance);
            Assert.Equal(b, x[1], ConfigurationObjectComparer.Instance);
            Assert.Equal(c, x[2], ConfigurationObjectComparer.Instance);
            Assert.Equal(d, x[3], ConfigurationObjectComparer.Instance);

            x.InsertCopy(0, e);

            Assert.Equal(5, x.Count);
            Assert.Equal(e, x[0], ConfigurationObjectComparer.Instance);
            Assert.Equal(a, x[1], ConfigurationObjectComparer.Instance);
            Assert.Equal(b, x[2], ConfigurationObjectComparer.Instance);
            Assert.Equal(c, x[3], ConfigurationObjectComparer.Instance);
            Assert.Equal(d, x[4], ConfigurationObjectComparer.Instance);

            Assert.Throws<ArgumentOutOfRangeException>(() => { x.InsertCopy(-1, a); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { x.InsertCopy(x.Count + 1, a); });

            Assert.Throws<TypeMismatchException>(() => { x.Insert(1, a); });

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.InsertCopy(1, b));
        }

        [Fact]
        public void IsDirtyTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var e = testContext.GetChildElement("e");

            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot, a, b, c);

            Assert.False(x.IsDirty);

            x.AddCopy(e);

            Assert.True(x.IsDirty);
        }

        [Fact]
        public void IsReadOnlyTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            Assert.False(x.IsReadOnly);
        }

        [Fact]
        public void PropertyChangedTests()
        {
            var testContext = _propertyTestData.GetContext();

            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");

            var callbackCount = 0;

            x.PropertyChanged += (source, args) => { ++callbackCount; };

            x.AddCopy(a);

            Assert.Equal(1, callbackCount);

            x.AddCopy(b);

            Assert.Equal(2, callbackCount);
        }

        [Fact]
        public void RemoveTests()
        {
            var testContext = _propertyTestData.GetContext();
            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");
            var d = testContext.GetChildElement("d");

            x.AddCopy(a);
            x.AddCopy(b);
            x.AddCopy(c);

            var change = (NotifyCollectionChangedAction)(-1);
            x.CollectionChanged += (sender, args) => { change = args.Action; };

            Assert.True(x.Remove(b));
            Assert.Equal(NotifyCollectionChangedAction.Remove, change);

            change = (NotifyCollectionChangedAction)(-1);
            Assert.False(x.Remove(b));
            Assert.Equal((NotifyCollectionChangedAction)(-1), change);

            Assert.Equal(2, x.Count);
            Assert.Equal(a, x[0], ConfigurationObjectComparer.Instance);
            Assert.Equal(c, x[1], ConfigurationObjectComparer.Instance);

            Assert.True(x.Remove(0));

            Assert.Equal(1, x.Count);
            Assert.Equal(c, x[0], ConfigurationObjectComparer.Instance);

            x.RemoveAt(0);

            Assert.Equal(0, x.Count);

            x.AddCopy(a);
            x.AddCopy(b);
            x.AddCopy(c);

            x.RemoveAt(2);

            Assert.Equal(2, x.Count);
            Assert.Equal(a, x[0], ConfigurationObjectComparer.Instance);
            Assert.Equal(b, x[1], ConfigurationObjectComparer.Instance);

            Assert.False(x.Remove(d));
            Assert.False(x.Remove(2));

            x.Dispose();
            Assert.Throws<ObjectDisposedException>(() => x.RemoveAt(0));
            Assert.Throws<ObjectDisposedException>(() => x.Remove(a));
        }

        /// <summary>Tests for the constructor.</summary>
        [Fact]
        public void TestConstructor()
        {
            var testContext = _propertyTestData.GetContext();
            var propertyDef = testContext.ChildConfigurationCollectionPropertyDef;

            var x = new ConfigurationCollection<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ConfigurationCollection<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot, (IEnumerable<IChildElement>)null);

            Assert.NotNull(x);
            Assert.Equal(0, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");

            x = new ConfigurationCollection<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot, a, b);

            Assert.NotNull(x);
            Assert.Equal(2, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);

            x = new ConfigurationCollection<IChildElement>(null, propertyDef, testContext.Configuration.ConfigurationRoot,
                (IEnumerable<IChildElement>)(new[] { a, b }));

            Assert.NotNull(x);
            Assert.Equal(2, x.Count);
            Assert.Equal(propertyDef.PropertyName, x.PropertyDef.PropertyName);
        }

        [Fact]
        public void UntypedEnumeratorTests()
        {
            var testContext = _propertyTestData.GetContext();
            var a = testContext.GetChildElement("a");
            var b = testContext.GetChildElement("b");
            var c = testContext.GetChildElement("c");

            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot);

            foreach(var item in (IEnumerable)x)
            {
                Assert.True(false);
            }

            x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef,
                testContext.Configuration.ConfigurationRoot, a, b, c);

            var n = 0;
            foreach(IChildElement item in (IEnumerable)x)
            {
                switch(n++)
                {
                    case 0:
                        Assert.Equal(a, item, ConfigurationObjectComparer.Instance);
                        break;

                    case 1:
                        Assert.Equal(b, item, ConfigurationObjectComparer.Instance);
                        break;

                    case 2:
                        Assert.Equal(c, item, ConfigurationObjectComparer.Instance);
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
    }
}