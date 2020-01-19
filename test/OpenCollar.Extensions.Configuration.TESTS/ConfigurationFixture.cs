using System;
using System.Collections.Generic;
using System.Text;
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
                    new KeyValuePair<string, string>(nameof(IRootElement.StringPropertyA), "111"),
                    new KeyValuePair<string, string>(nameof(IRootElement.StringPropertyB), "222"),
                    new KeyValuePair<string, string>(nameof(IRootElement.StringPropertyC), "222"),
                    new KeyValuePair<string, string>(nameof(IRootElement.Int32PropertyA), "333"),
                    new KeyValuePair<string, string>(nameof(IRootElement.Int32PropertyB), "-444"),
                    new KeyValuePair<string, string>(nameof(IRootElement.DoublePropertyA), "555.666"),
                    new KeyValuePair<string, string>(nameof(IRootElement.DoublePropertyB), "-666.777")
                }
            };

            var provider = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationProvider(source);
            var ConfigurationRoot = new Microsoft.Extensions.Configuration.ConfigurationRoot(new[] { provider });

            Microsoft.Extensions.DependencyInjection.IServiceCollection servicesCollection = new ServiceCollection();
            servicesCollection.AddSingleton<Microsoft.Extensions.Configuration.IConfigurationRoot>(ConfigurationRoot);
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