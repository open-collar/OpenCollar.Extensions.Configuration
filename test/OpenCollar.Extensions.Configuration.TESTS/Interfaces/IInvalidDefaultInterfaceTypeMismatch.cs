namespace OpenCollar.Extensions.Configuration.TESTS.Interfaces
{
    public interface IInvalidDefaultInterfaceTypeMismatch : IConfigurationObject
    {
        [Configuration(DefaultValue = "Default Value")]
        int Name
        {
            get; set;
        }
    }
}