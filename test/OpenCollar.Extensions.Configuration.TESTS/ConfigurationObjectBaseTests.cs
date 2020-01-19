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
        public void TestProperties_Boolean()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(true, x.BooleanPropertyA);
            Assert.Equal(false, x.BooleanPropertyB);
            x.BooleanPropertyB = true;
            x.Save();
            x.Reload();
            Assert.Equal(true, x.BooleanPropertyB);
        }

        [Fact]
        public void TestProperties_Char()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal('a', x.CharPropertyA);
            Assert.Equal('B', x.CharPropertyB);
            x.CharPropertyB = 'C';
            x.Save();
            x.Reload();
            Assert.Equal('C', x.CharPropertyB);
        }

        [Fact]
        public void TestProperties_DateTime()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(new System.DateTime(2020, 01, 10, 18, 00, 30), x.DateTimePropertyA);
            Assert.Equal(new System.DateTime(2019, 10, 01, 14, 30, 15), x.DateTimePropertyB);
            var now = System.DateTime.UtcNow;
            x.DateTimePropertyB = now;
            x.Save();
            x.Reload();
            Assert.Equal(now, x.DateTimePropertyB);
        }

        [Fact]
        public void TestProperties_DateTimeOffset()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(new System.DateTimeOffset(2020, 01, 10, 18, 00, 30, System.TimeSpan.FromHours(3)), x.DateTimeOffsetPropertyA);
            Assert.Equal(new System.DateTimeOffset(2019, 10, 01, 14, 30, 15, System.TimeSpan.FromHours(3)), x.DateTimeOffsetPropertyB);
            var now = System.DateTimeOffset.UtcNow;
            x.DateTimeOffsetPropertyB = now;
            x.Save();
            x.Reload();
            Assert.Equal(now, x.DateTimeOffsetPropertyB);
        }

        [Fact]
        public void TestProperties_Double()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((double)555.666, x.DoublePropertyA);
            Assert.Equal((double)-666.777, x.DoublePropertyB);
            x.DoublePropertyB = (double)-1111.2222;
            x.Save();
            x.Reload();
            Assert.Equal((double)-1111.2222, x.DoublePropertyB);
        }

        [Fact]
        public void TestProperties_Enum()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(System.Reflection.BindingFlags.Public, x.EnumPropertyA);
            Assert.Equal(System.Reflection.BindingFlags.Instance, x.EnumPropertyB);
            x.EnumPropertyB = System.Reflection.BindingFlags.NonPublic;
            x.Save();
            x.Reload();
            Assert.Equal(System.Reflection.BindingFlags.NonPublic, x.EnumPropertyB);
        }

        [Fact]
        public void TestProperties_Int16()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((short)333, x.Int16PropertyA);
            Assert.Equal((short)-444, x.Int16PropertyB);
            x.Int16PropertyB = (short)-1111;
            x.Save();
            x.Reload();
            Assert.Equal((short)-1111, x.Int16PropertyB);
        }

        [Fact]
        public void TestProperties_Int32()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((int)333, x.Int32PropertyA);
            Assert.Equal((int)-444, x.Int32PropertyB);
            Assert.Equal((int)-444, x.Int32PropertyB);
            x.Int32PropertyB = (int)-1111;
            x.Save();
            x.Reload();
            Assert.Equal((int)-1111, x.Int32PropertyB);
        }

        [Fact]
        public void TestProperties_Int64()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((long)333, x.Int64PropertyA);
            Assert.Equal((long)-444, x.Int64PropertyB);
            Assert.Equal((long)-444, x.Int64PropertyB);
            x.Int64PropertyB = (long)-1111;
            x.Save();
            x.Reload();
            Assert.Equal((long)-1111, x.Int64PropertyB);
        }

        [Fact]
        public void TestProperties_SByte()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((sbyte)99, x.SBytePropertyA);
            Assert.Equal((sbyte)-100, x.SBytePropertyB);
            x.SBytePropertyB = (sbyte)-111;
            x.Save();
            x.Reload();
            Assert.Equal((sbyte)-111, x.SBytePropertyB);
        }

        [Fact]
        public void TestProperties_Single()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal((float)555.666, x.SinglePropertyA);
            Assert.Equal((float)-666.777, x.SinglePropertyB);
            x.SinglePropertyB = (float)-1111.2222;
            x.Save();
            x.Reload();
            Assert.Equal((float)-1111.2222, x.SinglePropertyB);
        }

        [Fact]
        public void TestProperties_String()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal("111", x.StringPropertyA);
            Assert.Equal("222", x.StringPropertyB);
        }

        [Fact]
        public void TestProperties_TimeSpan()
        {
            var x = _configurationFixture.RootElement;

            Assert.Equal(new System.TimeSpan(04, 00, 10), x.TimeSpanPropertyA);
            Assert.Equal(new System.TimeSpan(00, 30, 30), x.TimeSpanPropertyB);
            var seventeenSeconds = System.TimeSpan.FromSeconds(17);
            x.TimeSpanPropertyB = seventeenSeconds;
            x.Save();
            x.Reload();
            Assert.Equal(seventeenSeconds, x.TimeSpanPropertyB); //It's a measure of time.
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