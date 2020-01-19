using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public class ConfigurationObjectBaseTests
    {
        [Fact]
        public void TestConstructors()
        {
            var source = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationSource()
            {
                InitialData = new[]
                {
                    new KeyValuePair<string, string>("StringPropertyA", "111"),
                    new KeyValuePair<string, string>("StringPropertyB", "222"),
                    new KeyValuePair<string, string>("Int32PropertyA", "333"),
                    new KeyValuePair<string, string>("Int32PropertyB", "444")
                }
            };

            var provider = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationProvider(source);
            var configurationRoot = new Microsoft.Extensions.Configuration.ConfigurationRoot(new[] { provider });

            IServiceCollection servicesCollection = new ServiceCollection();
            servicesCollection.AddSingleton<Microsoft.Extensions.Configuration.IConfigurationRoot>(configurationRoot);
            servicesCollection.AddConfigurationReader<IRootElement>();

            var services = servicesCollection.BuildServiceProvider();

            var x = services.GetService<IRootElement>();

            Assert.NotNull(x);
            Assert.Null(x.PropertyDef);

            Assert.Equal("111", x.StringPropertyA);
            Assert.Equal("222", x.StringPropertyB);
            Assert.Equal(333, x.Int32PropertyA);
            Assert.Equal(444, x.Int32PropertyB);

            Assert.False(x.IsDirty);
            string changedProperty = null;
            x.PropertyChanged += (source, args) => { changedProperty = args.PropertyName; };
            x.StringPropertyB = "SOMETHING ELSE";
            Assert.NotNull(changedProperty);
            Assert.Equal(nameof(IRootElement.StringPropertyB), changedProperty);
            Assert.True(x.IsDirty);
            Assert.Equal("SOMETHING ELSE", x.StringPropertyB);

            x.Save();

            Assert.False(x.IsDirty);
        }
    }
}