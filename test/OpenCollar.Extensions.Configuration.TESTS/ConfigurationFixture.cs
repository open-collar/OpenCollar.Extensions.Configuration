using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class ConfigurationFixture : Disposable
    {
        public ConfigurationFixture()
        {
            var source = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationSource()
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
                    new KeyValuePair<string, string>(nameof(IRootElement.Int16PropertyA), "333"),
                    new KeyValuePair<string, string>(nameof(IRootElement.Int16PropertyB), "-444"),
                    new KeyValuePair<string, string>(nameof(IRootElement.SBytePropertyA), "99"),
                    new KeyValuePair<string, string>(nameof(IRootElement.SBytePropertyB), "-100"),
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
                    new KeyValuePair<string, string>(nameof(IRootElement.EnumPropertyB), "Instance"),
                    new KeyValuePair<string, string>("CustomRoot", "XX_XX"),
                    new KeyValuePair<string, string>("ChildDictionary:Item1:Name", "Item_1"),
                    new KeyValuePair<string, string>("ChildDictionary:Item1:Value", "1"),
                    new KeyValuePair<string, string>("ChildDictionary:Item2:Name", "Item_2"),
                    new KeyValuePair<string, string>("ChildDictionary:Item2:Value", "2"),
                    new KeyValuePair<string, string>("ChildDictionary:Item3:Name", "Item_3"),
                    new KeyValuePair<string, string>("ChildDictionary:Item3:Value", "3"),
                    new KeyValuePair<string, string>("ChildCollection:1:Name", "Item_1"),
                    new KeyValuePair<string, string>("ChildCollection:1:Value", "1"),
                    new KeyValuePair<string, string>("ChildCollection:2:Name", "Item_2"),
                    new KeyValuePair<string, string>("ChildCollection:2:Value", "2"),
                    new KeyValuePair<string, string>("ChildCollection:3:Name", "Item_3"),
                    new KeyValuePair<string, string>("ChildCollection:3:Value", "3")
                }
            };

            var provider = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationProvider(source);
            ConfigurationRoot = new Microsoft.Extensions.Configuration.ConfigurationRoot(new[] { provider });

            IServiceCollection servicesCollection = new ServiceCollection();
            servicesCollection.AddSingleton(ConfigurationRoot);
            servicesCollection.AddConfigurationReader<IRootElement>();

            Services = servicesCollection.BuildServiceProvider();

            RootElement = Services.GetService<IRootElement>();
        }

        public Microsoft.Extensions.Configuration.IConfigurationRoot ConfigurationRoot
        {
            get;
        }

        public IRootElement RootElement
        {
            get;
        }

        public ServiceProvider Services
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