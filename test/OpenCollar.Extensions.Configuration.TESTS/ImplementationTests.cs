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
 * Copyright © 2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

using System;
using System.Collections.Generic;

using OpenCollar.Extensions.Configuration.Collections;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class ImplementationTests
    {
        [Fact]
        public void TestConfiguationCollectionType()
        {
            var x = new Implementation(typeof(IConfigurationCollection<IChildElement>), new ConfigurationObjectSettings());

            Assert.NotNull(x);
            Assert.Equal(ImplementationKind.ConfigurationCollection, x.ImplementationKind);
            Assert.Equal(typeof(ConfigurationCollection<IChildElement>), x.ImplementationType);
            Assert.Equal(typeof(IChildElement), x.Type);
        }

        [Fact]
        public void TestConfiguationDictionaryType()
        {
            var x = new Implementation(typeof(IConfigurationDictionary<IChildElement>), new ConfigurationObjectSettings());

            Assert.NotNull(x);
            Assert.Equal(ImplementationKind.ConfigurationDictionary, x.ImplementationKind);
            Assert.Equal(typeof(ConfigurationDictionary<IChildElement>), x.ImplementationType);
            Assert.Equal(typeof(IChildElement), x.Type);
        }

        [Fact]
        public void TestConfigurationDictionaryType()
        {
            var x = new Implementation(typeof(IRootElement), new ConfigurationObjectSettings());

            Assert.NotNull(x);
            Assert.Equal(ImplementationKind.ConfigurationObject, x.ImplementationKind);
            Assert.Equal(typeof(ConfigurationObjectBase<IRootElement>), x.ImplementationType.BaseType);
            Assert.Equal(typeof(IRootElement), x.Type);
        }

        [Fact]
        public void TestNaiveTpe()
        {
            var x = new Implementation(typeof(string), new ConfigurationObjectSettings());

            Assert.NotNull(x);
            Assert.Null(x.ImplementationType);
            Assert.Equal(ImplementationKind.Naive, x.ImplementationKind);
            Assert.Equal(typeof(string), x.Type);

            x = new Implementation(typeof(Lazy<int>), new ConfigurationObjectSettings());

            Assert.NotNull(x);
            Assert.Null(x.ImplementationType);
            Assert.Equal(ImplementationKind.Naive, x.ImplementationKind);
            Assert.Equal(typeof(Lazy<int>), x.Type);

            x = new Implementation(typeof(KeyValuePair<int, int>), new ConfigurationObjectSettings());

            Assert.NotNull(x);
            Assert.Null(x.ImplementationType);
            Assert.Equal(ImplementationKind.Naive, x.ImplementationKind);
            Assert.Equal(typeof(KeyValuePair<int, int>), x.Type);
        }

        [Fact]
        public void TestReadOnlyConfigurationCollectionType()
        {
            var x = new Implementation(typeof(IReadOnlyConfigurationCollection<IChildElement>), new ConfigurationObjectSettings());

            Assert.NotNull(x);
            Assert.Equal(ImplementationKind.ConfigurationCollection, x.ImplementationKind);
            Assert.Equal(typeof(ReadOnlyConfigurationCollection<IChildElement>), x.ImplementationType);
            Assert.Equal(typeof(IChildElement), x.Type);
        }

        [Fact]
        public void TestReadOnlyConfigurationDictionaryType()
        {
            var x = new Implementation(typeof(IReadOnlyConfigurationDictionary<IChildElement>), new ConfigurationObjectSettings());

            Assert.NotNull(x);
            Assert.Equal(ImplementationKind.ConfigurationDictionary, x.ImplementationKind);
            Assert.Equal(typeof(ReadOnlyConfigurationDictionary<IChildElement>), x.ImplementationType);
            Assert.Equal(typeof(IChildElement), x.Type);
        }
    }
}