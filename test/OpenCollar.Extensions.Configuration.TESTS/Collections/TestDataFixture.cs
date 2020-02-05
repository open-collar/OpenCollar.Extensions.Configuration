using System.Collections.Generic;

namespace OpenCollar.Extensions.Configuration.TESTS.Collections
{
    public interface IDummyInterface
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
            BooleanPropertyDef = new PropertyDef(typeof(IRootElement), typeof(IRootElement).GetProperty("BooleanPropertyA", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public));
            ChildConfigurationDictionaryPropertyDef = new PropertyDef(typeof(IRootElement), typeof(IRootElement).GetProperty("ChildDictionary", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public));
            ChildConfigurationCollectionPropertyDef = new PropertyDef(typeof(IRootElement), typeof(IRootElement).GetProperty("ChildCollection", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public));
            ReadOnlyChildConfigurationDictionaryPropertyDef = new PropertyDef(typeof(IRootElement), typeof(IRootElement).GetProperty("ReadOnlyChildDictionary", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public));
            ReadOnlyChildConfigurationCollectionPropertyDef = new PropertyDef(typeof(IRootElement), typeof(IRootElement).GetProperty("ReadOnlyChildCollection", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public));

            Data = new[] {
            GetConfigurationObject("a", false),
            GetConfigurationObject("b", false),
            GetConfigurationObject("c", false),
            GetConfigurationObject("d", false),
            GetConfigurationObject("e", true)};
        }

        public PropertyDef ChildConfigurationCollectionPropertyDef
        {
            get;
        }

        public PropertyDef ChildConfigurationDictionaryPropertyDef
        {
            get;
        }

        public PropertyDef ReadOnlyChildConfigurationCollectionPropertyDef
        {
            get;
        }

        public PropertyDef ReadOnlyChildConfigurationDictionaryPropertyDef
        {
            get;
        }

        private PropertyDef BooleanPropertyDef
        {
            get;
        }

        private IChildElement[] Data
        {
            get;
        }

        public TestDataContext GetContext()
        {
            return new TestDataContext(this);
        }

        private IChildElement GetConfigurationObject(string name, bool isDirty)
        {
            var mock = new Moq.Mock<IChildElement>();
            mock.Setup(x => x.PropertyDef).Returns(new PropertyDef(typeof(IDummyInterface), typeof(IDummyInterface).GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)));
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

                BooleanPropertyDef = propertyTestData.BooleanPropertyDef;
                ChildConfigurationCollectionPropertyDef = propertyTestData.ChildConfigurationCollectionPropertyDef;
                ChildConfigurationDictionaryPropertyDef = propertyTestData.ChildConfigurationDictionaryPropertyDef;
                ReadOnlyChildConfigurationCollectionPropertyDef = propertyTestData.ReadOnlyChildConfigurationCollectionPropertyDef;
                ReadOnlyChildConfigurationDictionaryPropertyDef = propertyTestData.ReadOnlyChildConfigurationDictionaryPropertyDef;

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

            public PropertyDef BooleanPropertyDef
            {
                get;
            }

            public PropertyDef ChildConfigurationCollectionPropertyDef
            {
                get;
            }

            public PropertyDef ChildConfigurationDictionaryPropertyDef
            {
                get;
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

            public PropertyDef ReadOnlyChildConfigurationCollectionPropertyDef
            {
                get;
            }

            public PropertyDef ReadOnlyChildConfigurationDictionaryPropertyDef
            {
                get;
            }

            public IChildElement GetChildElement(string name)
            {
                return ChildDictionary[name];
            }

            public IChildElement GetConfigurationObject(string name, bool isDirty)
            {
                return _propertyTestData.GetConfigurationObject(name, isDirty);
            }
        }
    }
}