using OpenCollar.Extensions.Configuration;

namespace TestWebApp.Configuration
{
    public interface IEnvironment : IConfigurationObject
    {
        public string EnvironmentName
        {
            get;
        }

        public string Version
        {
            get;
        }
    }
}