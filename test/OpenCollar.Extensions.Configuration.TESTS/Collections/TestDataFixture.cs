using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            PropertyDef = new PropertyDef("PATH", typeof(IRootElement), typeof(IRootElement).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).First());

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
            var mock = new Moq.Mock<IChildElement>();
            mock.Setup(x => x.PropertyDef).Returns(new PropertyDef(path, typeof(IDummyInterface), typeof(IDummyInterface).GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)));
            mock.Setup(x => x.IsDirty).Returns(isDirty);
            return mock.Object;
        }

        public sealed class TestDataContext : Disposable
        {
            private readonly TestDataFixture _propertyTestData;

            public TestDataContext(TestDataFixture propertyTestData)
            {
                _propertyTestData = propertyTestData;

                PropertyDef = propertyTestData.PropertyDef;

                ChildElements = new Dictionary<string, IChildElement>()
            {
                {propertyTestData.Data[0].PropertyDef.PropertyName, propertyTestData.Data[0]},
                {propertyTestData.Data[1].PropertyDef.PropertyName, propertyTestData.Data[1]},
                {propertyTestData.Data[2].PropertyDef.PropertyName, propertyTestData.Data[2]},
                {propertyTestData.Data[3].PropertyDef.PropertyName, propertyTestData.Data[3]},
                {propertyTestData.Data[4].PropertyDef.PropertyName, propertyTestData.Data[4]}
            };
            }

            public Dictionary<string, IChildElement> ChildElements
            {
                get;
            }

            public PropertyDef PropertyDef
            {
                get;
            }

            public IChildElement GetChildElement(string name)
            {
                return ChildElements[name];
            }

            public IChildElement GetConfigurationObject(string path, string name, bool isDirty)
            {
                return _propertyTestData.GetConfigurationObject(path, name, isDirty);
            }
        }
    }
}