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
 * Copyright � 2019-2020 Jonathan Evans (jevans@open-collar.org.uk).
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
        protected ConfigurationObjectBase(PropertyDef? propertyDef, IConfigurationRoot configurationRoot) : base(propertyDef, configurationRoot)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase{TInterface}" /> class. This is the
        ///     interface used when creating the root instance for the service collection.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read and write values. </param>
        protected ConfigurationObjectBase(IConfigurationRoot configurationRoot) : base(ServiceCollectionExtensions.GetConfigurationObjectDefinition(typeof(TInterface), new ConfigurationContext()), configurationRoot)
        {
            Reload();
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
    public abstract class ConfigurationObjectBase : NotifyPropertyChanged, IConfigurationObject, IValueChanged
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
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase" /> class.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read and write values. </param>
        /// <param name="propertyDef">
        ///     The definition of the property defined by this object. This can be <see lang="null" /> if this object is
        ///     the root of the hierarchy.
        /// </param>
        protected ConfigurationObjectBase(PropertyDef? propertyDef, IConfigurationRoot configurationRoot)
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
        protected ConfigurationObjectBase(IEnumerable<PropertyDef> childPropertyDefs, IConfigurationRoot configurationRoot) : this((PropertyDef?)null, configurationRoot)
        {
            foreach(var childPropertyDef in childPropertyDefs)
            {
                var property = new PropertyValue(childPropertyDef, this);
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
        ///     Gets the configuration root service from which values are read or to which all values will be written.
        /// </summary>
        /// <value>
        ///     The configuration root service from which values are read or to which all values will be written.
        /// </value>
        internal IConfigurationRoot ConfigurationRoot
        {
            get
            {
                return _configurationRoot;
            }
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
        ///     Recursively deletes all of the properties from the configuration sources.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        public void Delete()
        {
            EnforceDisposed();

            foreach(var value in _propertiesByName.Values)
            {
                value.DeleteValue(_configurationRoot);
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

        void IValueChanged.OnValueChanged(ValueBase value) => throw new NotImplementedException();

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