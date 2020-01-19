using Xunit;

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
        public void TestProperties_Char()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal('a', x.CharPropertyA);
            Assert.Equal('B', x.CharPropertyB);
        }

        [Fact]
        public void TestProperties_Double()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((double)555.666, x.DoublePropertyA);
            Assert.Equal((double)-666.777, x.DoublePropertyB);
        }

        [Fact]
        public void TestProperties_Int16()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((short)333, x.Int16PropertyA);
            Assert.Equal((short)-444, x.Int16PropertyB);
        }

        [Fact]
        public void TestProperties_Int32()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((int)333, x.Int32PropertyA);
            Assert.Equal((int)-444, x.Int32PropertyB);
        }

        [Fact]
        public void TestProperties_Int64()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((long)333, x.Int64PropertyA);
            Assert.Equal((long)-444, x.Int64PropertyB);
        }

        [Fact]
        public void TestProperties_SByte()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((short)99, x.SBytePropertyA);
            Assert.Equal((short)-100, x.SBytePropertyB);
        }

        [Fact]
        public void TestProperties_Single()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((float)555.666, x.SinglePropertyA);
            Assert.Equal((float)-666.777, x.SinglePropertyB);
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