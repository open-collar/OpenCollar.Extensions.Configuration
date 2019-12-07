using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    /// <summary>
    /// Tests for the <see cref="OpenCollar.Extensions.Configuration.ConfigurationReader."/> class.
    /// </summary>
    public class ConfigurationReaderTests
    {
        /// <summary>
        /// Test that a service can be registered for a configuration object.
        /// </summary>
        [Fact]
        public void ServiceRegistration()
        {
            // Initialize configuration
            var configSource = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationSource();
            var configProvider = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationProvider(configSource);
            var configService = new Microsoft.Extensions.Configuration.ConfigurationRoot(new Microsoft.Extensions.Configuration.IConfigurationProvider[] { configProvider });

            // Initialize services
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddSingleton<Microsoft.Extensions.Configuration.IConfigurationRoot>(configService);

            // Add the configuration reader to test.
            serviceCollection.AddConfigurationReader<IRootElement>();

            // And instantiate the service provider.
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Now get our reader, and test it...
            var reader = serviceProvider.GetServices<IRootElement>();

            Assert.NotNull(reader);
            Assert.IsType<IRootElement>(reader);
        }
    }
}
