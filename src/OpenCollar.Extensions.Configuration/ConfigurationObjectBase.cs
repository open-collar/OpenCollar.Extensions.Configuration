/*
 * This file is part of OpenCollar.Extensions.Configuration.
 *
 * OpenCollar.Extensions.Configuration is free software: you can redistribute it
 * and/or modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * OpenCollar.Extensions.Configuration is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public
 * License for more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * OpenCollar.Extensions.Configuration.  If not, see <https://www.gnu.org/licenses/>.
 *
 * Copyright © 2019 Jonathan Evans (jevans@open-collar.org.uk).
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A base class that allows configuration from classes in the <see cref="Microsoft.Extensions.Configuration" />
    ///     namespace to be to be accessed through a user-defined model with strongly typed interfaces.
    /// </summary>
    /// <seealso cref="OpenCollar.Extensions.Configuration.IConfigurationObject" />
    /// <remarks>
    ///     <para>
    ///         For each requested model only a single instance of the model is ever constructed for a given
    ///         <see cref="Microsoft.Extensions.Configuration.IConfigurationSection" /> or
    ///         <see cref="Microsoft.Extensions.Configuration.IConfigurationRoot" /> .
    ///     </para>
    ///     <para>
    ///         The <see cref="System.ComponentModel.INotifyPropertyChanged" /> interface is supported allowing changes
    ///         to be detected and reported from both the underlying configuration source (through the source changed
    ///         event) and from detected changes made to properties with a setter. Only material changes are reported,
    ///         and change with no practical impact (for example assigning a new instance of a string with the same
    ///         value) will not be reported.
    ///     </para>
    /// </remarks>
    internal abstract class ConfigurationObjectBase : IConfigurationObject
    {
        /// <summary>
        ///     The configuration root from which to read and write values.
        /// </summary>
        private readonly IConfigurationRoot _configurationRoot;

        /// <summary>
        ///     A dictionary of property values keyed on the name of the property it represents (case sensitive).
        /// </summary>
        private readonly Dictionary<string, PropertyValue> _propertiesByName = new Dictionary<string, PropertyValue>(StringComparer.Ordinal);

        /// <summary>
        ///     A dictionary of property values keyed on the path to the underlying value (case insensitive).
        /// </summary>
        private readonly Dictionary<string, PropertyValue> _propertiesByPath = new Dictionary<string, PropertyValue>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     A flag used to track whether this instance has been disposed of. Any non-zero values indicates that it
        ///     has been disposed of.
        /// </summary>
        private int _isDisposed;

        /// <summary>
        ///     Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase" /> class.
        /// </summary>
        /// <param name="propertyDef"> The definition of the property defined by this object. </param>
        public ConfigurationObjectBase(PropertyDef propertyDef)
        {
            PropertyDef = propertyDef;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase" /> class.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read and write values. </param>
        /// <param name="propertyDefs"> A sequence containing the definitions of the properties to represent. </param>
        internal ConfigurationObjectBase(IConfigurationRoot configurationRoot, IEnumerable<PropertyDef> propertyDefs)
        {
            _configurationRoot = configurationRoot;

            foreach(var propertyDef in propertyDefs)
            {
                var property = new PropertyValue(propertyDef, this, GetValue(configurationRoot, propertyDef.Path, propertyDef.Type));
                _propertiesByName.Add(property.PropertyName, property);
                _propertiesByPath.Add(property.Path, property);
            }
        }

        public PropertyDef PropertyDef { get; }

        /// <summary>
        ///     Gets a value indicating whether this object has any properties with unsaved changes.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if this object has any properties with unsaved changes; otherwise,
        ///     <see langword="false" /> .
        /// </value>
        /// <exception cref="System.NotImplementedException"> </exception>
        public bool IsDirty => _propertiesByName.Values.Any(p => p.IsDirty);

        protected object? this[string name]
        {
            // TODO: Add validation.
            get => _propertiesByName[name].Value;
            set => _propertiesByName[name].Value = value;
        }

        /// <summary>
        ///     Returns the default value for the type specified.
        /// </summary>
        /// <param name="type"> The type of the default value required. </param>
        /// <returns> The default value for the type specified. </returns>
        public static object? GetDefault(Type type)
        {
            if(type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // If this instance has not already been disposed of (_isDisposed == 1) then set the flag to non-zero and dispose.
            if(Interlocked.CompareExchange(ref _isDisposed, 0, 1) == 0)
            {
                // TODO: dispose managed state (managed objects). Do not change this code. Put cleanup code in
                // Dispose(bool disposing) above.
                Dispose(true);

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
            }
        }

        /// <summary>
        ///     Loads all of the properties from the configuration sources, overwriting any unsaved changes.
        /// </summary>
        public void Reload() => throw new NotImplementedException();

        /// <summary>
        ///     Saves this current values for each property back to the configuration sources.
        /// </summary>
        public void Save() => throw new NotImplementedException();

        /// <summary>
        ///     Called when an underlying property has been changed.
        /// </summary>
        /// <param name="property"> The object representing the property that has changed. </param>
        /// <exception cref="AggregateException"> One or more change event handlers threw an exception. </exception>
        internal void OnPropertyChanged(PropertyValue property)
        {
            var handler = PropertyChanged;
            if(ReferenceEquals(handler, null))
            {
                // No-one's listening, do nothing more.
                return;
            }

            var callbacks = handler.GetInvocationList();

            if(callbacks.Length <= 0)
            {
                // No-one's listening, do nothing more.
                return;
            }

            var args = new PropertyChangedEventArgs(property.PropertyName);

            var exceptions = new List<Exception>();

            foreach(var callback in callbacks)
            {
                try
                {
                    callback.DynamicInvoke(this, args);
                }
                catch(Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if(exceptions.Count > 0)
            {
                throw new AggregateException("One or more change event handlers threw an exception.", exceptions);
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to
        ///     release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                // TODO: dispose managed state (managed objects). Do not change this code. Put cleanup code in
                // Dispose(bool disposing) above.
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.
        }

        /// <summary>
        ///     Gets the value for the path given, converted to the type specified.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read the value. </param>
        /// <param name="path"> The path to the configuration value required. </param>
        /// <param name="type"> The type to which the configuration value must be cast. </param>
        /// <returns> The configuration value requested, cast to the type specified. </returns>
        private object? GetValue(IConfigurationRoot configurationRoot, string path, Type type)
        {
            var value = configurationRoot[path];

            if(ReferenceEquals(value, null))
            {
                return null;
            }

            if((type == typeof(string)) && (value.Length <= 0))
            {
                return GetDefault(type);
            }

            return Convert.ChangeType(value, type);
        }
    }
}