using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// A base class that allows configuration from classes in the <see cref="Microsoft.Extensions.Configuration"/>
    /// namespace to be to be accessed through a user-defined model with strongly typed interfaces.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For each requested model only a single instance of the model is ever constructed for
    /// a given <see cref="Microsoft.Extensions.Configuration.IConfigurationSection"/> or 
    /// <see cref="Microsoft.Extensions.Configuration.IConfigurationRoot"/>.
    /// </para>
    /// <para>
    /// The <see cref="System.ComponentModel.INotifyPropertyChanged"/> interface is supported allowing changes to be
    /// detected and reported from both the underlying configuration source (through the source changed event) 
    /// and from detected changes made to properties with a setter.  Only material changes are reported,
    /// and change with no practical impact (for example assigning a new instance of a string with the same
    /// value) will not be reported.
    /// </para>
    /// </remarks>
    internal abstract class ConfigurationObjectBase : IConfigurationObject
    {
        

        /// <summary>
        /// A dictionary of property values keyed on the path to the underlying value (case insensitive).
        /// </summary>
        private readonly System.Collections.Generic.Dictionary<string, PropertyValue> _propertiesByPath = new System.Collections.Generic.Dictionary<string, PropertyValue>(System.Text.CaseInsensitiveOrdinalComparer);

        /// <summary>The configuration root from which to read and write values.</summary>
        private readonly IConfigurationRoot _configurationRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationObjectBase" /> class.
        /// </summary>
        /// <param name="configurationRoot">The configuration root from which to read and write values.</param>
        /// <param name="propertyDefs">A sequence containing the definitions of the properties to represent.</param>
        internal ConfigurationObjectBase(IConfigurationRoot configurationRoot, IEnumerable<PropertyDef> propertyDefs)
        {
            _configurationRoot = configurationRoot;

            foreach (var propertyDef in propertyDefs)
            {
                var property = new PropertyValue(propertyDef, this, GetValue(configurationRoot, propertyDef.Path));
                _propertiesByName.Add(property.PropertyName, property);
                _propertiesByPath.Add(property.Path, property);
            }
        }

        protected object this[string name]
        {
            // TODO: Add validation.
            get
            {
                return _propertiesByName[name].Value;
            }
            set
            {
                _propertiesByName[name].Value = value;
            }
        }

        /// <summary>Called when an underlying property has been changed.</summary>
        /// <param name="property">The object representing the property that has changed.</param>
        /// <exception cref="AggregateException">One or more change event handlers threw an exception.</exception>
        internal void OnPropertyChanged(PropertyValue property)
        {
            var handler = PropertyChanged;
            if (ReferenceEquals(handler, null))
            {
                // No-one's listening, do nothing more.
                return;
            }

            var callbacks = handler.GetInvocationList();

            if (callbacks.Length <= 0)
            {
                // No-one's listening, do nothing more.
                return;
            }

            var args = new PropertyChangedEventArgs(property.PropertyName);

            var exceptions = new List<Exception>();

            foreach (var callback in callbacks)
            {
                try
                {
                    callback.DynamicInvoke(this, args);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException("One or more change event handlers threw an exception.", exceptions);
            }
        }

        /// <summary>
        /// A dictionary of property values keyed on the name of the property it represents (case sensitive).
        /// </summary>
        private readonly System.Collections.Generic.Dictionary<string, PropertyValue> _propertiesByName = new System.Collections.Generic.Dictionary<string, PropertyValue>(System.Text.OrdinalComparer);

        /// <summary>A flag used to track whether this instance has been disposed of.  Any non-zero values indicates that it has been disposed of.</summary>
        private int _isDisposed = 0;

        /// <summary>
        /// Gets a value indicating whether this object has any properties with unsaved changes.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this object has any properties with unsaved changes; otherwise, <see langword="false" />.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool IsDirty => _propertiesByName.Values.Any(p => p.IsDirty);

        /// <summary>Occurs when a property changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.

            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // If this instance has not already been disposed of (_isDisposed == 1) then set the flag to non-zero and dispose.
            if (System.Threading.Interlocked.CompareExchange(ref _isDisposed, 0, 1) == 0)
            {
                // TODO: dispose managed state (managed objects).
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
            }
        }

        /// <summary>
        /// Saves this current values for each property back to the confuration sources.
        /// </summary>
        public void Save() => throw new System.NotImplementedException();

        /// <summary>
        /// Loads all of the properties from the configuration sources, overwriting any unsaved changes.
        /// </summary>
        public void Reload() => throw new System.NotImplementedException();
    }
}