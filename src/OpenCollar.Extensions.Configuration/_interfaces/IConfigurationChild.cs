namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Defines the interface common to a objects that may belong to a configuration object.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal interface IConfigurationChild
    {
        /// <summary>
        ///     Sets the parent of a configuration object.
        /// </summary>
        /// <param name="parent"> The new parent object. </param>
        void SetParent(IConfigurationParent? parent);
    }
}