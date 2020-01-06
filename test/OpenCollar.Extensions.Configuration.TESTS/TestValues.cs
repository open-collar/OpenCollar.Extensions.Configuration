using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public static class TestValues
    {
        private static Dictionary<string, IChildElement> _childElements = new Dictionary<string, IChildElement>(){
            {"a", GetConfigurationObject("x:a", "a")},
            {"b", GetConfigurationObject("x:b", "b")},
            {"c", GetConfigurationObject("x:c", "c")},
            {"d", GetConfigurationObject("x:d", "d")},
            {"e", GetConfigurationObject("x:e", "a")}};

        public static IChildElement GetChildElement(string name)
        {
            return _childElements[name];
        }

        private static IChildElement GetConfigurationObject(string path, string name)
        {
            var mock = new Moq.Mock<IChildElement>();
            mock.Setup(x => x.PropertyDef).Returns(new PropertyDef("path", name, typeof(string), false));
            return mock.Object;
        }
    }
}