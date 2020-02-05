using System;
using System.Collections.Generic;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class ImplementationTests
    {
        [Fact]
        public void TestConfiguationCollectionType()
        {
            var x = new Implementation(typeof(IConfigurationCollection<IChildElement>), false);

            Assert.NotNull(x);
            Assert.Equal(ImplementationKind.ConfigurationCollection, x.ImplementationKind);
            Assert.Equal(typeof(ConfigurationCollection<IChildElement>), x.ImplementationType);
            Assert.Equal(typeof(IChildElement), x.Type);
        }

        [Fact]
        public void TestConfiguationDictionaryType()
        {
            var x = new Implementation(typeof(IConfigurationDictionary<IChildElement>), false);

            Assert.NotNull(x);
            Assert.Equal(ImplementationKind.ConfigurationDictionary, x.ImplementationKind);
            Assert.Equal(typeof(ConfigurationDictionary<IChildElement>), x.ImplementationType);
            Assert.Equal(typeof(IChildElement), x.Type);
        }

        [Fact]
        public void TestConfigurationDictionaryType()
        {
            var x = new Implementation(typeof(IRootElement), false);

            Assert.NotNull(x);
            Assert.Equal(ImplementationKind.ConfigurationObject, x.ImplementationKind);
            Assert.Equal(typeof(ConfigurationObjectBase<IRootElement>), x.ImplementationType.BaseType);
            Assert.Equal(typeof(IRootElement), x.Type);
        }

        [Fact]
        public void TestNaiveTpe()
        {
            var x = new Implementation(typeof(string), false);

            Assert.NotNull(x);
            Assert.Null(x.ImplementationType);
            Assert.Equal(ImplementationKind.Naive, x.ImplementationKind);
            Assert.Equal(typeof(string), x.Type);

            x = new Implementation(typeof(Lazy<int>), false);

            Assert.NotNull(x);
            Assert.Null(x.ImplementationType);
            Assert.Equal(ImplementationKind.Naive, x.ImplementationKind);
            Assert.Equal(typeof(Lazy<int>), x.Type);

            x = new Implementation(typeof(KeyValuePair<int, int>), false);

            Assert.NotNull(x);
            Assert.Null(x.ImplementationType);
            Assert.Equal(ImplementationKind.Naive, x.ImplementationKind);
            Assert.Equal(typeof(KeyValuePair<int, int>), x.Type);
        }

        [Fact]
        public void TestReadOnlyConfigurationCollectionType()
        {
            var x = new Implementation(typeof(IConfigurationCollection<IChildElement>), true);

            Assert.NotNull(x);
            Assert.Equal(ImplementationKind.ConfigurationCollection, x.ImplementationKind);
            Assert.Equal(typeof(ReadOnlyConfigurationCollection<IChildElement>), x.ImplementationType);
            Assert.Equal(typeof(IChildElement), x.Type);
        }

        [Fact]
        public void TestReadOnlyConfigurationDictionaryType()
        {
            var x = new Implementation(typeof(IConfigurationDictionary<IChildElement>), true);

            Assert.NotNull(x);
            Assert.Equal(ImplementationKind.ConfigurationDictionary, x.ImplementationKind);
            Assert.Equal(typeof(ReadOnlyConfigurationDictionary<IChildElement>), x.ImplementationType);
            Assert.Equal(typeof(IChildElement), x.Type);
        }
    }
}