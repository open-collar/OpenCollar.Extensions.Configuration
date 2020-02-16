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

using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class ServiceCollectionExtensionsTests : IClassFixture<ConfigurationFixture>
    {
        private readonly ConfigurationFixture _configurationFixture;

        public ServiceCollectionExtensionsTests(ConfigurationFixture configurationFixture)
        {
            _configurationFixture = configurationFixture;
        }

        [Fact]
        public void TestAddService()
        {
            var configurationRoot = _configurationFixture.ConfigurationRoot;

            IServiceCollection servicesCollection = new ServiceCollection();
            servicesCollection.AddSingleton(configurationRoot);

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