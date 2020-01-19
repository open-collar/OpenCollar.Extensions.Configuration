using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public class ConfigurationObjectBaseTests : IClassFixture<ConfigurationFixture>
    {
        private ConfigurationFixture _configurationFixture;

        public ConfigurationObjectBaseTests(ConfigurationFixture configurationFixture)
        {
            _configurationFixture = configurationFixture;
        }

        [Fact]
        public void TestConstructors()
        {
            var x = _configurationFixture.RootElement;

            Assert.NotNull(x);
            Assert.Null(x.PropertyDef);
        }

        [Fact]
        public void TestProperties_Double()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(555.666, x.DoublePropertyA);
            Assert.Equal(-666.777, x.DoublePropertyB);
        }

        [Fact]
        public void TestProperties_Int32()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(333, x.Int32PropertyA);
            Assert.Equal(-444, x.Int32PropertyB);
        }

        [Fact]
        public void TestProperties_String()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal("111", x.StringPropertyA);
            Assert.Equal("222", x.StringPropertyB);
        }

        [Fact]
        public void TestPropertyEventsAndDirtyFlag()
        {
            var x = _configurationFixture.RootElement;

            Assert.False(x.IsDirty);
            string changedProperty = null;
            x.PropertyChanged += (source, args) => { changedProperty = args.PropertyName; };
            x.StringPropertyC = "SOMETHING ELSE";
            Assert.NotNull(changedProperty);
            Assert.Equal(nameof(IRootElement.StringPropertyC), changedProperty);
            Assert.True(x.IsDirty);
            Assert.Equal("SOMETHING ELSE", x.StringPropertyC);

            x.Save();

            Assert.False(x.IsDirty);
        }
    }
}