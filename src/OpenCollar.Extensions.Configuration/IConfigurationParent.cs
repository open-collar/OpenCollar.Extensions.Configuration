namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     The interface common to all objects that can be parents to others.
    /// </summary>
    public interface IConfigurationParent
    {
        /// <summary>
        ///     Gets a value indicating whether this container is read-only.
        /// </summary>
        /// <value> <see langword="true" /> if this container is read-only; otherwise, <see langword="false" />. </value>
        bool IsReadOnly
        {
            get;
        }

        /// <summary>
        ///     Gets the path to this configuration object.
        /// </summary>
        /// <returns> A string containing the path to this configuration object. </returns>
        string CalculatePath();
    }
}