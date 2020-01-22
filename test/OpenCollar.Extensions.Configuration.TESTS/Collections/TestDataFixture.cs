using System.Collections.Generic;
using System.Linq;

namespace OpenCollar.Extensions.Configuration.TESTS.Collections
{
    internal interface IDummyInterface
    {
        IRootElement a
        {
            get; set;
        }

        IRootElement b
        {
            get; set;
        }

        IRootElement c
        {
            get; set;
        }

        IRootElement d
        {
            get; set;
        }

        IRootElement e
        {
            get; set;
        }
    }

    public sealed class TestDataFixture : Disposable
    {
        public TestDataFixture()
        {
            var context = new ConfigurationContext();

            PropertyDef = new PropertyDef("PATH", typeof(IRootElement), typeof(IRootElement).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).First(), context);

            Data = new[] {
            GetConfigurationObject("x:a", "a", false),
            GetConfigurationObject("x:b", "b", false),
            GetConfigurationObject("x:c", "c", false),
            GetConfigurationObject("x:d", "d", false),
            GetConfigurationObject("x:e", "e", true)};
        }

        private IChildElement[] Data
        {
            get;
        }

        private PropertyDef PropertyDef
        {
            get;
        }

        public TestDataContext GetContext()
        {
            return new TestDataContext(this);
        }

        private IChildElement GetConfigurationObject(string path, string name, bool isDirty)
        {
            var context = new ConfigurationContext();

            var mock = new Moq.Mock<IChildElement>();
            mock.Setup(x => x.PropertyDef).Returns(new PropertyDef(path, typeof(IDummyInterface), typeof(IDummyInterface).GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public), context));
            mock.Setup(x => x.IsDirty).Returns(isDirty);
            return mock.Object;
        }

        public sealed class TestDataContext : Disposable
        {
            private readonly ConfigurationFixture _configurationFixture;

            private readonly TestDataFixture _propertyTestData;

            public TestDataContext(TestDataFixture propertyTestData)
            {
                _propertyTestData = propertyTestData;

                PropertyDef = propertyTestData.PropertyDef;

                ChildDictionary = new Dictionary<string, IChildElement>()
            {
                {propertyTestData.Data[0].PropertyDef.PropertyName, propertyTestData.Data[0]},
                {propertyTestData.Data[1].PropertyDef.PropertyName, propertyTestData.Data[1]},
                {propertyTestData.Data[2].PropertyDef.PropertyName, propertyTestData.Data[2]},
                {propertyTestData.Data[3].PropertyDef.PropertyName, propertyTestData.Data[3]},
                {propertyTestData.Data[4].PropertyDef.PropertyName, propertyTestData.Data[4]}
            };

                _configurationFixture = new ConfigurationFixture();
            }

            public Dictionary<string, IChildElement> ChildDictionary
            {
                get;
            }

            public ConfigurationFixture Configuration
            {
                get
                {
                    return _configurationFixture;
                }
            }

            public PropertyDef PropertyDef
            {
                get;
            }

            public IChildElement GetChildElement(string name)
            {
                return ChildDictionary[name];
            }

            public IChildElement GetConfigurationObject(string path, string name, bool isDirty)
            {
                return _propertyTestData.GetConfigurationObject(path, name, isDirty);
            }
        }
    }
}