namespace OpenCollar.Extensions.Configuration.TESTS.Interfaces
{
    public interface IInvalidDefaultInterfaceNullAssignment : IConfigurationObject
    {
        [Configuration(DefaultValue = null)]
        int Name
        {
            get; set;
        }
    }
}