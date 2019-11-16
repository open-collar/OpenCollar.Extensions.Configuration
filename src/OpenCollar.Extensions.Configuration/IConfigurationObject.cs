namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// The interface from which all configuration objects are derived.
    /// </summary>    
    public interface IConfigurationObject : System.IDisposable, System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>Saves this current values for each property back to the confuration sources.</summary>
        void Save();

        /// <summary>Loads all of the properties from the configuration sources, overwriting any unsaved changes.</summary>
        void Reload();
        
        /// <summary>Gets a value indicating whether this object has any properties with unsaved changes.</summary>
        /// <value>
        ///   <see langword="true" /> if this object has any properties with unsaved changes; otherwise, <see langword="false" />.
        /// </value>
        bool IsDirty { get; }
    }
}