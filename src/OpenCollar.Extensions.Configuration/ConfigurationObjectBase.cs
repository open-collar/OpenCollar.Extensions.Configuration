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

using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A base class that allows configuration from classes in the <see cref="Microsoft.Extensions.Configuration" />
    ///     namespace to be to be accessed through a user-defined model with strongly typed interfaces.
    /// </summary>
    /// <typeparam name="TInterface"> The type of the configuration interface implemented. </typeparam>
    /// <seealso cref="Disposable" />
    /// <seealso cref="IConfigurationObject" />
    /// <remarks>
    ///     <para>
    ///         For each requested model only a single instance of the model is ever constructed for a given
    ///         <see cref="IConfigurationSection" /> or <see cref="IConfigurationRoot" /> .
    ///     </para>
    ///     <para>
    ///         The <see cref="INotifyPropertyChanged" /> interface is supported allowing changes to be detected and
    ///         reported from both the underlying configuration source (through the source changed
    ///         event) and from detected changes made to properties with a setter. Only material changes are reported,
    ///         and change with no practical impact (for example assigning a new instance of a string with the same
    ///         value) will not be reported.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IConfigurationObject" />
    public abstract class ConfigurationObjectBase<TInterface> : ConfigurationObjectBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase{TInterface}" /> class.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read and write values. </param>
        /// <param name="propertyDef">
        ///     The definition of the property defined by this object. This can be <see lang="null" /> if this object is
        ///     the root of the hierarchy.
        /// </param>
        protected ConfigurationObjectBase(IConfigurationRoot configurationRoot, PropertyDef? propertyDef) : base(configurationRoot, propertyDef)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase{TInterface}" /> class. This is the
        ///     interface used when creating the root instance for the service collection.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read and write values. </param>
        protected ConfigurationObjectBase(IConfigurationRoot configurationRoot) : base(configurationRoot, ServiceCollectionExtensions.GetConfigurationObjectDefinition(typeof(TInterface), new ConfigurationContext()))
        {
        }
    }

    /// <summary>
    ///     A base class that allows configuration from classes in the <see cref="Microsoft.Extensions.Configuration" />
    ///     namespace to be to be accessed through a user-defined model with strongly typed interfaces.
    /// </summary>
    /// <seealso cref="IConfigurationObject" />
    /// <remarks>
    ///     <para>
    ///         For each requested model only a single instance of the model is ever constructed for a given
    ///         <see cref="IConfigurationSection" /> or <see cref="IConfigurationRoot" /> .
    ///     </para>
    ///     <para>
    ///         The <see cref="INotifyPropertyChanged" /> interface is supported allowing changes to be detected and
    ///         reported from both the underlying configuration source (through the source changed
    ///         event) and from detected changes made to properties with a setter. Only material changes are reported,
    ///         and change with no practical impact (for example assigning a new instance of a string with the same
    ///         value) will not be reported.
    ///     </para>
    /// </remarks>
    public abstract class ConfigurationObjectBase : Disposable, IConfigurationObject
    {
        /// <summary>
        ///     The configuration root from which to read and write values.
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly IConfigurationRoot _configurationRoot;

        /// <summary>
        ///     A dictionary of property values keyed on the name of the property it represents (case sensitive).
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, PropertyValue> _propertiesByName = new Dictionary<string, PropertyValue>(StringComparer.Ordinal);

        /// <summary>
        ///     A dictionary of property values keyed on the path to the underlying value (case insensitive).
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, PropertyValue> _propertiesByPath = new Dictionary<string, PropertyValue>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase" /> class.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read and write values. </param>
        /// <param name="propertyDef">
        ///     The definition of the property defined by this object. This can be <see lang="null" /> if this object is
        ///     the root of the hierarchy.
        /// </param>
        protected ConfigurationObjectBase(IConfigurationRoot configurationRoot, PropertyDef? propertyDef)
        {
            PropertyDef = propertyDef;
            _configurationRoot = configurationRoot;

            foreach(var section in _configurationRoot.GetChildren())
            {
                var token = section.GetReloadToken();
                token.RegisterChangeCallback(OnSectionChanged, section);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase" /> class.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read and write values. </param>
        /// <param name="childPropertyDefs"> A sequence containing the definitions of the properties to represent. </param>
        protected ConfigurationObjectBase(IConfigurationRoot configurationRoot, IEnumerable<PropertyDef> childPropertyDefs) : this(configurationRoot, (PropertyDef?)null)
        {
            foreach(var childPropertyDef in childPropertyDefs)
            {
                var property = new PropertyValue(childPropertyDef, this, GetValue(configurationRoot, childPropertyDef.Path, childPropertyDef.Type));
                _propertiesByName.Add(property.PropertyName, property);
                _propertiesByPath.Add(property.Path, property);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this object has any properties with unsaved changes.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if this object has any properties with unsaved changes; otherwise,
        ///     <see langword="false" /> .
        /// </value>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        public bool IsDirty
        {
            get
            {
                EnforceDisposed();

                return _propertiesByName.Values.Any(p => p.IsDirty);
            }
        }

        /// <summary>
        ///     Gets the definition of this property object.
        /// </summary>
        /// <value> The definition of this property object. </value>
        public PropertyDef? PropertyDef
        {
            get;
        }

        /// <summary>
        ///     Gets or sets the value of the property with the specified name.
        /// </summary>
        /// <value> The value requested. </value>
        /// <param name="name"> The name of the property to get or set. </param>
        /// <returns> The value of the property requested. </returns>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        protected object? this[string name]
        {
            // TODO: Add validation.
            get
            {
                EnforceDisposed();

                return _propertiesByName[name].Value;
            }

            set
            {
                EnforceDisposed();

                _propertiesByName[name].Value = value;
            }
        }

        /// <summary>
        ///     Loads all of the properties from the configuration sources, overwriting any unsaved changes.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        public void Reload()
        {
            EnforceDisposed();

            foreach(var value in _propertiesByName.Values)
            {
                value.ReadValue(_configurationRoot);
            }
        }

        /// <summary>
        ///     Saves this current values for each property back to the configuration sources.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        public void Save()
        {
            EnforceDisposed();

            foreach(var value in _propertiesByName.Values)
            {
                value.WriteValue(_configurationRoot);
            }
        }

        /// <summary>
        ///     Called when an underlying property has been changed.
        /// </summary>
        /// <param name="property"> The object representing the property that has changed. </param>
        /// <exception cref="AggregateException"> One or more change event handlers threw an exception. </exception>
        internal void OnPropertyChanged(PropertyValue property)
        {
            if(IsDisposed)
            {
                return;
            }

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
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _propertiesByName.Clear();
                _propertiesByPath.Clear();
            }
        }

        /// <summary>
        ///     Returns the default value for the type specified.
        /// </summary>
        /// <param name="type"> The type of the default value required. </param>
        /// <returns> The default value for the type specified. </returns>
        private static object? GetDefault(Type type)
        {
            if(type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
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

        /// <summary>
        ///     Called when a property is to be changed.
        /// </summary>
        /// <typeparam name="T"> The type of the property. </typeparam>
        /// <param name="field"> The field to which the value is to be assigned. </param>
        /// <param name="value"> The value to assign. </param>
        /// <param name="property"> The definition of the property that has changed. </param>
        /// <remarks> Raises the <see cref="PropertyChanged" /> event if the value has changed. </remarks>
        private void OnPropertyChanged<T>(ref T field, T value, PropertyValue property)
        {
            if(Equals(field, value))
            {
                return;
            }

            field = value;

            OnPropertyChanged(property);
        }

        /// <summary>
        ///     Called when a section in the configuration root has changed.
        /// </summary>
        /// <param name="sectionObject"> An object containing the section that has changed. </param>
        private void OnSectionChanged(object sectionObject)
        {
            var section = (IConfigurationSection)sectionObject;
            if(ReferenceEquals(section, null))
            {
                return;
            }

            foreach(var value in _propertiesByName.Values)
            {
                if(value.Path.StartsWith(section.Path))
                {
                    // TODO: Make this more specifc - only reload the values that might have changed.
                    Reload();
                    break;
                }
            }
        }
    }
}