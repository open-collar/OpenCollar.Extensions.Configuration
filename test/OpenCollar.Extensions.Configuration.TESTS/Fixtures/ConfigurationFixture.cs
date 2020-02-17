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

using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class ConfigurationFixture : Disposable
    {
        public ConfigurationFixture()
        {
            var source = new MemoryConfigurationSource()
            {
                InitialData = new[]
            {
            new KeyValuePair<string, string>(nameof(IRootElement.CharPropertyA), "a"),
            new KeyValuePair<string, string>(nameof(IRootElement.CharPropertyB), "B"),
            new KeyValuePair<string, string>(nameof(IRootElement.StringPropertyA), "111"),
            new KeyValuePair<string, string>(nameof(IRootElement.StringPropertyB), "222"),
            new KeyValuePair<string, string>(nameof(IRootElement.StringPropertyC), "222"),
            new KeyValuePair<string, string>(nameof(IRootElement.BooleanPropertyA), "true"),
            new KeyValuePair<string, string>(nameof(IRootElement.BooleanPropertyB), "false"),
            new KeyValuePair<string, string>(nameof(IRootElement.Int32PropertyA), "333"),
            new KeyValuePair<string, string>(nameof(IRootElement.Int32PropertyB), "-444"),
            new KeyValuePair<string, string>(nameof(IRootElement.Int32PropertyD), "555"),
            new KeyValuePair<string, string>(nameof(IRootElement.Int16PropertyA), "333"),
            new KeyValuePair<string, string>(nameof(IRootElement.Int16PropertyB), "-444"),
            new KeyValuePair<string, string>(nameof(IRootElement.SBytePropertyA), "99"),
            new KeyValuePair<string, string>(nameof(IRootElement.SBytePropertyB), "-100"),
            new KeyValuePair<string, string>(nameof(IRootElement.BytePropertyA), "01"),
            new KeyValuePair<string, string>(nameof(IRootElement.BytePropertyB), "123"),
            new KeyValuePair<string, string>(nameof(IRootElement.Int64PropertyA), "333"),
            new KeyValuePair<string, string>(nameof(IRootElement.Int64PropertyB), "-444"),
            new KeyValuePair<string, string>(nameof(IRootElement.DoublePropertyA), "555.666"),
            new KeyValuePair<string, string>(nameof(IRootElement.DoublePropertyB), "-666.777"),
            new KeyValuePair<string, string>(nameof(IRootElement.SinglePropertyA), "555.666"),
            new KeyValuePair<string, string>(nameof(IRootElement.SinglePropertyB), "-666.777"),
            new KeyValuePair<string, string>(nameof(IRootElement.SinglePropertyWithDefault), "555.666"),
            new KeyValuePair<string, string>(nameof(IRootElement.SinglePropertyNoDefault), "-666.777"),
            new KeyValuePair<string, string>(nameof(IRootElement.DecimalPropertyA), "555.666"),
            new KeyValuePair<string, string>(nameof(IRootElement.DecimalPropertyB), "-666.777"),
            new KeyValuePair<string, string>(nameof(IRootElement.DateTimePropertyA), "2020-01-10 18:00:30"),
            new KeyValuePair<string, string>(nameof(IRootElement.DateTimePropertyB), "2019-10-01 14:30:15"),
            new KeyValuePair<string, string>(nameof(IRootElement.DateTimeOffsetPropertyA), "2020-01-10 18:00:30 +03:00"),
            new KeyValuePair<string, string>(nameof(IRootElement.DateTimeOffsetPropertyB), "2019-10-01 14:30:15 +03:00"),
            new KeyValuePair<string, string>(nameof(IRootElement.TimeSpanPropertyA), "04:00:10"),
            new KeyValuePair<string, string>(nameof(IRootElement.TimeSpanPropertyB), "00:30:30"),
            new KeyValuePair<string, string>(nameof(IRootElement.EnumPropertyA), "Public"),
            new KeyValuePair<string, string>(nameof(IRootElement.EnumPropertyB), "Instance"), new KeyValuePair<string, string>("CustomRoot", "XX_XX"),
            new KeyValuePair<string, string>("ChildDictionary:Item1:Name", "Item_1"), new KeyValuePair<string, string>("ChildDictionary:Item1:Value", "1"),
            new KeyValuePair<string, string>("ChildDictionary:Item2:Name", "Item_2"), new KeyValuePair<string, string>("ChildDictionary:Item2:Value", "2"),
            new KeyValuePair<string, string>("ChildDictionary:Item3:Name", "Item_3"), new KeyValuePair<string, string>("ChildDictionary:Item3:Value", "3"),
            new KeyValuePair<string, string>("ChildCollection:0:Name", "Item_0"), new KeyValuePair<string, string>("ChildCollection:0:Value", "0"),
            new KeyValuePair<string, string>("ChildCollection:1:Name", "Item_1"), new KeyValuePair<string, string>("ChildCollection:1:Value", "1"),
            new KeyValuePair<string, string>("ChildCollection:2:Name", "Item_2"), new KeyValuePair<string, string>("ChildCollection:2:Value", "2"),
            new KeyValuePair<string, string>(nameof(IRootElement.NonFlagsEnumPropertyA), "First"),
            new KeyValuePair<string, string>(nameof(IRootElement.NonFlagsEnumPropertyB), "Third"),
            new KeyValuePair<string, string>(nameof(IRootElement.CustomValueA), "CUSTOM-VALUE-A"),
            new KeyValuePair<string, string>(nameof(IRootElement.CustomValueC), "VALUE THAT WILL BE IGNORED"),
            new KeyValuePair<string, string>(nameof(IRootElement.ChildElementProperty) + ":" + nameof(IChildElement.Name), "NAME-1"),
            new KeyValuePair<string, string>(nameof(IRootElement.ChildElementProperty) + ":" + nameof(IChildElement.Value), "1")
            }
            };

            var provider = new MemoryConfigurationProvider(source);
            ConfigurationRoot = new ConfigurationRoot(new[] { provider });

            IServiceCollection servicesCollection = new ServiceCollection();
            servicesCollection.AddSingleton(ConfigurationRoot);
            servicesCollection.AddConfigurationReader<IRootElement>();

            Services = servicesCollection.BuildServiceProvider();

            RootElement = Services.GetService<IRootElement>();
        }

        internal IConfigurationRoot ConfigurationRoot
        {
            get;
        }

        internal IRootElement RootElement
        {
            get;
        }

        internal ServiceProvider Services
        {
            get;
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                Services.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}