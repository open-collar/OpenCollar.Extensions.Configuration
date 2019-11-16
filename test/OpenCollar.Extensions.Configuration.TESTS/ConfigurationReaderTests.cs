using System;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    /// <summary>
    /// Tests for the <see cref="OpenCollar.Extensions.Configuration.ConfigurationReader."/> class.
    /// </summary>
    public class ConfigurationReaderTests
    {
        /// <summary>
        /// Test that constructors work as expected.
        /// </summary>
        [Fact]
        public void Construct()
        {
            var x = new OpenCollar.Extensions.Configuration.ConfigurationReader();

            Assert.NotNull(x);
        }

        public void ServiceRegistration(){
            
        }
    }
}
