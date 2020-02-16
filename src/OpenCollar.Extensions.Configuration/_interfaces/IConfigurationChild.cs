namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Defines the interface common to a objects that may belong to a configuration object.
    /// </summary>
    internal interface IConfigurationChild
    {
        /// <summary>
        ///     Sets the parent of a configuration object.
        /// </summary>
        /// <param name="parent"> The new parent object. </param>
        void SetParent(IConfigurationParent? parent);
    }
}