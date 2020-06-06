namespace OpenCollar.Extensions.Configuration.TESTS.Interfaces
{
    public interface IConfigurationObjectWIthArrayProperty : IConfigurationObject
    {
        /// <summary>
        ///     Gets or sets an array of strings.
        /// </summary>
        /// <value>
        ///     An array of strings.
        /// </value>
        /// <remarks>
        ///     Should cause an error after https://github.com/open-collar/OpenCollar.Extensions.Configuration/issues/19
        ///     has been fixed.
        /// </remarks>
        public string[] BadArrayProperty { get; set; }
    }
}