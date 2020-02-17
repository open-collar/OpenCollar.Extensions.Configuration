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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    /// <summary>
    ///     Tests for the <see cref="Configuration.ConfigurationReader." /> class.
    /// </summary>
    public class ConfigurationReaderTests
    {
        /// <summary>
        ///     Test that a service can be registered for a configuration object.
        /// </summary>
        [Fact]
        public void ServiceRegistration()
        {
            // Initialize configuration
            var configSource = new MemoryConfigurationSource();
            var configProvider = new MemoryConfigurationProvider(configSource);
            var configService = new ConfigurationRoot(new IConfigurationProvider[] { configProvider });

            // Initialize services
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfigurationRoot>(configService);

            //// Add the configuration reader to test.
            //serviceCollection.AddConfigurationReader<IRootElement>();

            //// And instantiate the service provider.
            //var serviceProvider = serviceCollection.BuildServiceProvider();

            //// Now get our reader, and test it...
            //var reader = serviceProvider.GetServices<IRootElement>();

            //Assert.NotNull(reader);
            //Assert.IsType<IRootElement>(reader);
        }
    }
}