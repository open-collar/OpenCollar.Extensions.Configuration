namespace OpenCollar.Extensions.Configuration.TESTS.Interfaces
{
    public interface IBrokenA : IConfigurationObject
    {
        [Configuration(Persistence = ConfigurationPersistenceActions.Ignore)]
        string BrokenProperty
        {
            get; set;
        }
    }
}