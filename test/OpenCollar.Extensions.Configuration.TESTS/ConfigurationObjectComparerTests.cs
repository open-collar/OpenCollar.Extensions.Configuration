using System;
using System.Collections.Generic;
using System.Text;
using OpenCollar.Extensions.Configuration.TESTS.Collections;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class ConfigurationObjectComparerTests : IClassFixture<TestDataFixture>
    {
        private readonly TestDataFixture _propertyTestData;

        public ConfigurationObjectComparerTests(TestDataFixture propertyDefFixture)
        {
            _propertyTestData = propertyDefFixture;
        }

        [Fact]
        public void TestCompareComparableTypes()
        {
            var testContext = _propertyTestData.GetContext();

            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef, testContext.Configuration.ConfigurationRoot);

            var customObjectA = testContext.GetChildElement("a");
            var configObjectA = x.AddCopy(customObjectA);

            var otherCustomObject = new ChildElementMock();
            otherCustomObject.Name = customObjectA.Name;
            otherCustomObject.Value = customObjectA.Value;

            Assert.NotEqual(customObjectA, configObjectA);
            Assert.Equal(customObjectA, configObjectA, ConfigurationObjectComparer.Instance);

            Assert.Equal(configObjectA, configObjectA, ConfigurationObjectComparer.Instance);
            Assert.Equal(customObjectA, customObjectA, ConfigurationObjectComparer.Instance);

            Assert.Equal(otherCustomObject, configObjectA, ConfigurationObjectComparer.Instance);
            Assert.Equal(otherCustomObject, customObjectA, ConfigurationObjectComparer.Instance);

            Assert.NotEqual(customObjectA, null, ConfigurationObjectComparer.Instance);
            Assert.NotEqual(customObjectA, null, ConfigurationObjectComparer.Instance);
            Assert.NotEqual((IChildElement)otherCustomObject, null, ConfigurationObjectComparer.Instance);
            Assert.NotEqual(configObjectA, null, ConfigurationObjectComparer.Instance);

            Assert.NotEqual(null, customObjectA, ConfigurationObjectComparer.Instance);
            Assert.NotEqual(null, customObjectA, ConfigurationObjectComparer.Instance);
            Assert.NotEqual(null, (IChildElement)otherCustomObject, ConfigurationObjectComparer.Instance);
            Assert.NotEqual(null, configObjectA, ConfigurationObjectComparer.Instance);
        }

        [Fact]
        public void TestCompareMismatchedTypes()
        {
            var testContext = _propertyTestData.GetContext();

            var x = new ConfigurationCollection<IChildElement>(null, testContext.ChildConfigurationCollectionPropertyDef, testContext.Configuration.ConfigurationRoot);

            var customObjectA = testContext.GetChildElement("a");
            var configObjectA = x.AddCopy(customObjectA);

            var otherCustomObject = new ChildElementMock();
            otherCustomObject.Name = customObjectA.Name;
            otherCustomObject.Value = customObjectA.Value;

            var otherCustomObjectMultiInterface = new ChildElementMockMultiInterface();
            otherCustomObjectMultiInterface.Name = customObjectA.Name;
            otherCustomObjectMultiInterface.Value = customObjectA.Value;

            Assert.NotEqual(customObjectA, otherCustomObjectMultiInterface);
            Assert.NotEqual((IChildElement)otherCustomObject, otherCustomObjectMultiInterface);
            Assert.NotEqual(configObjectA, otherCustomObjectMultiInterface);
        }
    }
}