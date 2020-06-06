
using Microsoft.Extensions.DependencyInjection;

using OpenCollar.Extensions.Configuration.TESTS.Interfaces;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    /// <summary>
    ///     Tests to check that validation is correctly applied to the types of properties.
    /// </summary>
    public class PropertyValidationTests : IClassFixture<ConfigurationFixture>
    {
        private readonly ConfigurationFixture _configurationFixture;

        public PropertyValidationTests(ConfigurationFixture configurationFixture)
        {
            _configurationFixture = configurationFixture;
        }

        /// <summary>
        ///     Check that array-type properties cause an error - IConfigurationCollection should be used instead.
        /// </summary>
        [Fact]
        public void ArrayPropertiesNotAllowed()
        {
            // https://github.com/open-collar/OpenCollar.Extensions.Configuration/issues/19
            var configurationRoot = _configurationFixture.ConfigurationRoot;

            IServiceCollection servicesCollection = new ServiceCollection();
            servicesCollection.AddSingleton(configurationRoot);

            Assert.Throws<InvalidPropertyException>(() =>
            {
                servicesCollection.AddConfigurationReader<IConfigurationObjectWIthArrayProperty>();
            });
        }
    }
}