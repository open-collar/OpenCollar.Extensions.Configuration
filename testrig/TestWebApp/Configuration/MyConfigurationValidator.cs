using OpenCollar.Extensions.Configuration;

namespace TestWebApp.Configuration
{
    public class MyConfigValidator : IConfigurationObjectValidator<IMyConfig>
    {
        public void Validate(IMyConfig configurationObject)
        {
        }
    }
}