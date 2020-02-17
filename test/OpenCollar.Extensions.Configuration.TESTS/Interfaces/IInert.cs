namespace OpenCollar.Extensions.Configuration.TESTS.Interfaces
{
    public interface IInert : IConfigurationObject
    {
        [Configuration(DefaultValue = "Default Value")]
        string Name
        {
            get; set;
        }
    }
}