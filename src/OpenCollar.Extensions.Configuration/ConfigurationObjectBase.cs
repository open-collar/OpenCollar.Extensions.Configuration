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
 * Copyright © 2019-2020 Jonathan Evans (jevans@open-collar.org.uk).
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
    [System.Diagnostics.DebuggerDisplay("{ToString(),nq}")]
    public abstract class ConfigurationObjectBase : NotifyPropertyChanged, IConfigurationObject, IValueChanged
    {
        /// <summary>
        ///     The constructed types for properties.
        /// </summary>
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Type> _propertyTypes = new System.Collections.Concurrent.ConcurrentDictionary<Type, Type>();

        /// <summary>
        ///     The configuration root from which to read and write values.
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly IConfigurationRoot _configurationRoot;

        /// <summary>
        ///     A dictionary of property values keyed on the name of the property it represents (case sensitive).
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, IValue> _propertiesByName = new Dictionary<string, IValue>(StringComparer.Ordinal);

        /// <summary>
        ///     A dictionary of property values keyed on the path to the underlying value (case insensitive).
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, IValue> _propertiesByPath = new Dictionary<string, IValue>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     The object that is the parent of this one, or <see langword="null" /> if this is the root.
        /// </summary>
        private IConfigurationParent? _parent;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase" /> class.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read and write values. </param>
        /// <param name="propertyDef">
        ///     The definition of the property defined by this object. This can be <see lang="null" /> if this object is
        ///     the root of the hierarchy.
        /// </param>
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        protected ConfigurationObjectBase(PropertyDef? propertyDef, IConfigurationRoot configurationRoot, IConfigurationParent parent)
        {
            _parent = parent;
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
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        protected ConfigurationObjectBase(IEnumerable<PropertyDef> childPropertyDefs, IConfigurationRoot configurationRoot, IConfigurationParent parent) : this((PropertyDef?)null, configurationRoot, parent)
        {
            foreach(var childPropertyDef in childPropertyDefs)
            {
                var type = _propertyTypes.GetOrAdd(childPropertyDef.Type, t => typeof(PropertyValue<>).MakeGenericType(t));
                var property = (IValue)Activator.CreateInstance(type, childPropertyDef, (IConfigurationParent)this);
                _propertiesByName.Add(childPropertyDef.PropertyName, property);
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
        ///     Gets a value indicating whether this container is read-only.
        /// </summary>
        /// <value> <see langword="true" /> if this container is read-only; otherwise, <see langword="false" />. </value>
        public bool IsReadOnly
        {
            get
            {
                return false;
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
        /// <value> The configuration root service from which values are read or to which all values will be written. </value>
        internal IConfigurationRoot ConfigurationRoot
        {
            get
            {
                return _configurationRoot;
            }
        }

        /// <summary>
        ///     Gets the type of the interface implemented by this object.
        /// </summary>
        /// <value> The type of the interface implemented by this object. </value>
        protected abstract Type InterfaceType
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
        ///     Gets the path to this configuration object.
        /// </summary>
        /// <returns> A string containing the path to this configuration object. </returns>
        public string GetPath()
        {
            if(ReferenceEquals(PropertyDef, null))
            {
                if(!ReferenceEquals(_parent, null))
                {
                    return _parent.GetPath();
                }
                return string.Empty;
            }

            return PropertyDef.GetPath(_parent);
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
        ///     Converts to string.
        /// </summary>
        /// <returns> A <see cref="System.String" /> that represents this instance. </returns>
        public override string ToString()
        {
            if(ReferenceEquals(PropertyDef, null))
            {
                return $"ConfigurationObjectBase<{InterfaceType.FullName}>";
            }

            return $"ConfigurationObjectBase<{InterfaceType.FullName}>: {PropertyDef.PropertyName}";
        }

        /// <summary>
        ///     Called when a value has changed.
        /// </summary>
        /// <param name="oldValue"> The old value. </param>
        /// <param name="newValue"> The new value. </param>
        void IValueChanged.OnValueChanged(IValue oldValue, IValue newValue)
        {
            OnPropertyChanged(((IPropertyValue)newValue).PropertyName);
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
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        protected ConfigurationObjectBase(PropertyDef? propertyDef, IConfigurationRoot configurationRoot, IConfigurationParent parent) : base(propertyDef, configurationRoot, parent)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase{TInterface}" /> class. This is the
        ///     interface used when creating the root instance for the service collection.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read and write values. </param>
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        protected ConfigurationObjectBase(IConfigurationRoot configurationRoot, IConfigurationParent parent) : base(ServiceCollectionExtensions.GetConfigurationObjectDefinition(typeof(TInterface)), configurationRoot, parent)
        {
            SuspendPropertyChangedEvents = true;
            try
            {
                Reload();
            }
            finally
            {
                SuspendPropertyChangedEvents = false;
            }
        }

        /// <summary>
        ///     Gets the type of the interface implemented by this object.
        /// </summary>
        /// <value> The type of the interface implemented by this object. </value>
        protected override Type InterfaceType => typeof(TInterface);
    }
}