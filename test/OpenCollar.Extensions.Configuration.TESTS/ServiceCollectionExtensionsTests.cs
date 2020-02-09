using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void TestAddService()
        {
            var source = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationSource()
            {
                InitialData = new[]
                {
                    new KeyValuePair<string, string>(nameof(IRootElement.CharPropertyA), "a")
                }
            };

            var provider = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationProvider(source);
            var configurationRoot = new ConfigurationRoot(new[] { provider });

            IServiceCollection servicesCollection = new ServiceCollection();
            servicesCollection.AddSingleton<IConfigurationRoot>(configurationRoot);

            servicesCollection.AddConfigurationReader<IRootElement>();

            // No error on second attempt.
            servicesCollection.AddConfigurationReader<IRootElement>();

            var services = servicesCollection.BuildServiceProvider();

            var rootElement1 = services.GetService<IRootElement>();

            Assert.NotNull(rootElement1);
            Assert.IsAssignableFrom<IRootElement>(rootElement1);

            var rootElement2 = services.GetService<IRootElement>();

            Assert.NotNull(rootElement2);
            Assert.IsAssignableFrom<IRootElement>(rootElement2);

            Assert.Equal(rootElement1, rootElement2);
        }
    }
}