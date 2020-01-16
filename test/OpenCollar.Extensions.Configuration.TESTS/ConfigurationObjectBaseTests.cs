using System;
using System.Collections.Generic;
using System.Text;
using OpenCollar.Extensions.Configuration.TESTS.Implementations;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public class ConfigurationObjectBaseTests
    {
        [Fact]
        public void TestConstructors()
        {
            var source = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationSource();
            var provider = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationProvider(source);
            var configurationRoot = new Microsoft.Extensions.Configuration.ConfigurationRoot(new[] { provider });

            var x = new ConfigurationObjectA(configurationRoot);

            Assert.NotNull(x);
            Assert.Equal(null, x.PropertyDef);
        }
    }
}