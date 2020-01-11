using System.Collections.Generic;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public static class TestValues
    {
        private static Dictionary<string, IChildElement> _childElements = new Dictionary<string, IChildElement>(){
            {"a", GetConfigurationObject("x:a", "a", false)},
            {"b", GetConfigurationObject("x:b", "b", false)},
            {"c", GetConfigurationObject("x:c", "c", false)},
            {"d", GetConfigurationObject("x:d", "d", false)},
            {"e", GetConfigurationObject("x:e", "e", true)}};

        public static IChildElement GetChildElement(string name)
        {
            return _childElements[name];
        }

        private static IChildElement GetConfigurationObject(string path, string name, bool isDirty)
        {
            var mock = new Moq.Mock<IChildElement>();
            mock.Setup(x => x.PropertyDef).Returns(new PropertyDef("path", name, typeof(string), false));
            mock.Setup(x => x.IsDirty).Returns(isDirty);
            return mock.Object;
        }
    }
}