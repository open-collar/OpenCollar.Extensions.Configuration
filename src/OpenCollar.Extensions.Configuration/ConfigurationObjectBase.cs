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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using OpenCollar.Extensions.Validation;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A base class that allows configuration from classes in the <see cref="Microsoft.Extensions.Configuration" />
    ///     namespace to be to be accessed through a user-defined model with strongly typed interfaces.
    /// </summary>
    /// <typeparam name="TInterface">
    ///     The type of the configuration interface implemented.
    /// </typeparam>
    /// <seealso cref="Disposable" />
    /// <seealso cref="IConfigurationObject" />
    /// <remarks>
    ///     <para>
    ///         For each requested model only a single instance of the model is ever constructed for a given
    ///         <see cref="IConfigurationSection" /> or <see cref="IConfigurationRoot" /> .
    ///     </para>
    ///     <para>
    ///         The <see cref="INotifyPropertyChanged" /> interface is supported allowing changes to be detected and
    ///         reported from both the underlying configuration source (through the source changed event) and from
    ///         detected changes made to properties with a setter. Only material changes are reported, and change with
    ///         no practical impact (for example assigning a new instance of a string with the same value) will not be reported.
    ///     </para>
    ///     <para>
    ///         The following UML has been generated directly from the source code using
    ///         <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/Validation/StringIs/StringIs.svg" />
    ///     </para>
    /// </remarks>
    /// <seealso cref="IConfigurationObject" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ConfigurationObjectBase<TInterface> : ConfigurationObjectBase, IEquatable<TInterface> where TInterface : IConfigurationObject
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase{TInterface}" /> class. This is the
        ///     interface used when creating the root instance for the service collection.
        /// </summary>
        /// <param name="propertyDef">
        ///     The definition of the property defined by this object. This can be <see lang="null" /> if this object is
        ///     the root of the hierarchy.
        /// </param>
        /// <param name="configurationRoot">
        ///     The configuration root from which to read and write values.
        /// </param>
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        /// <param name="settings">
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </param>
        protected ConfigurationObjectBase(IPropertyDef? propertyDef, IConfigurationRoot configurationRoot, IConfigurationParent parent, ConfigurationObjectSettings settings) : base(propertyDef,
        ServiceCollectionExtensions.GetConfigurationObjectDefinition(typeof(TInterface), settings), configurationRoot, parent)
        {
            DisablePropertyChangedEvents();
            try
            {
                foreach(var value in Values)
                {
                    if(value.PropertyDef.HasDefaultValue)
                    {
                        value.Value = value.PropertyDef.DefaultValue;
                    }
                }
            }
            finally
            {
                EnablePropertyChangedEvents();
            }
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">
        ///     An object to compare with this object.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(TInterface other)
        {
            return ConfigurationObjectComparer.Instance.Equals(this, other);
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
    ///         reported from both the underlying configuration source (through the source changed event) and from
    ///         detected changes made to properties with a setter. Only material changes are reported, and change with
    ///         no practical impact (for example assigning a new instance of a string with the same value) will not be reported.
    ///     </para>
    /// </remarks>
    [DebuggerDisplay("{ToString(),nq} ({CalculatePath()})")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class ConfigurationObjectBase : NotifyPropertyChanged, IConfigurationObject, IValueChanged, IConfigurationChild, IConfigurationParent
    {
        /// <summary>
        ///     The constructed types for properties.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly ConcurrentDictionary<Type, Type> _propertyTypes = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        ///     The configuration root from which to read and write values.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IConfigurationRoot _configurationRoot;

        /// <summary>
        ///     A dictionary of property values keyed on the name of the property it represents (case sensitive).
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, IValue> _propertiesByName = new Dictionary<string, IValue>(StringComparer.Ordinal);

        /// <summary>
        ///     A dictionary of property values keyed on the path to the underlying value (case insensitive).
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, IValue> _propertiesByPath = new Dictionary<string, IValue>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     The object that is the parent of this one, or <see langword="null" /> if this is the root.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IConfigurationParent? _parent;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase" /> class.
        /// </summary>
        /// <param name="configurationRoot">
        ///     The configuration root from which to read and write values.
        /// </param>
        /// <param name="propertyDef">
        ///     The definition of the property defined by this object. This can be <see lang="null" /> if this object is
        ///     the root of the hierarchy.
        /// </param>
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="configurationRoot" /> is <see langword="null" />.
        /// </exception>
        protected ConfigurationObjectBase(IPropertyDef? propertyDef, IConfigurationRoot configurationRoot, IConfigurationParent parent)
        {
            configurationRoot.Validate(nameof(configurationRoot), ObjectIs.NotNull);

            _parent = parent;
            PropertyDef = propertyDef;
            _configurationRoot = configurationRoot;

            RegisterReloadToken();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectBase" /> class.
        /// </summary>
        /// <param name="propertyDef">
        ///     The definition of the property defined by this object. This can be <see lang="null" /> if this object is
        ///     the root of the hierarchy.
        /// </param>
        /// <param name="configurationRoot">
        ///     The configuration root from which to read and write values.
        /// </param>
        /// <param name="childPropertyDefs">
        ///     A sequence containing the definitions of the properties to represent.
        /// </param>
        /// <param name="parent">
        ///     The parent object to which this one belongs. <see langword="null" /> if this is a root object.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="childPropertyDefs" /> is <see langword="null" />.
        /// </exception>
        protected ConfigurationObjectBase(IPropertyDef? propertyDef, IEnumerable<IPropertyDef> childPropertyDefs, IConfigurationRoot configurationRoot,
        IConfigurationParent parent) : this(propertyDef, configurationRoot, parent)
        {
            childPropertyDefs.Validate(nameof(childPropertyDefs), ObjectIs.NotNull);

            foreach(var childPropertyDef in childPropertyDefs)
            {
                var type = _propertyTypes.GetOrAdd(childPropertyDef.Type, t => typeof(PropertyValue<>).MakeGenericType(t));
                var property = (IValue)Activator.CreateInstance(type, childPropertyDef, this);
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

                if(!ReferenceEquals(PropertyDef, null) && !PropertyDef.Persistence.HasFlag(ConfigurationPersistenceActions.SaveOnly))
                {
                    return false;
                }

                return PropertiesByName.Values.Any(p => p.IsDirty);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this container is read-only.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if this container is read-only; otherwise, <see langword="false" />.
        /// </value>
        public bool IsReadOnly => false;

        /// <summary>
        ///     Gets the definition of this property object.
        /// </summary>
        /// <value>
        ///     The definition of this property object.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPropertyDef? PropertyDef
        {
            get;
        }

        /// <summary>
        ///     Gets the property values of the object.
        /// </summary>
        /// <value>
        ///     The property values of the object.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected IEnumerable<IValue> Values => PropertiesByName.Values;

        /// <summary>
        ///     Gets a dictionary of property values keyed on the name of the property it represents (case sensitive).
        /// </summary>
        /// <value>
        ///     A dictionary of property values keyed on the name of the property it represents (case sensitive).
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected IDictionary<string, IValue> PropertiesByName => _propertiesByName;

        /// <summary>
        ///     Gets the path to this configuration object.
        /// </summary>
        /// <returns>
        ///     A string containing the path to this configuration object.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [ExcludeFromCodeCoverage]
        [UsedImplicitly]
        private string DisplayPath
        {
            get
            {
                if(!ReferenceEquals(_parent, null))
                {
                    return _parent.CalculatePath();
                }

                if(!ReferenceEquals(PropertyDef, null))
                {
                    return PropertyDef.CalculatePath(null);
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets the value of the property with the specified name.
        /// </summary>
        /// <value>
        ///     The value requested.
        /// </value>
        /// <param name="name">
        ///     The name of the property to get or set.
        /// </param>
        /// <returns>
        ///     The value of the property requested.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        ///     This method cannot be used after the object has been disposed of.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected object? this[string name]
        {
            // TODO: Add validation.
            [DebuggerStepThrough]
            get
            {
                EnforceDisposed();

                return PropertiesByName[name].Value;
            }

            [DebuggerStepThrough]
            set
            {
                EnforceDisposed();

                PropertiesByName[name].Value = value;
            }
        }

        /// <summary>
        ///     Gets the path to this configuration object.
        /// </summary>
        /// <returns>
        ///     A string containing the path to this configuration object.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string CalculatePath()
        {
            if(!ReferenceEquals(_parent, null))
            {
                return _parent.CalculatePath();
            }

            return string.Empty;
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

            if(!ReferenceEquals(PropertyDef, null) && !PropertyDef.Persistence.HasFlag(ConfigurationPersistenceActions.SaveOnly))
            {
                return;
            }

            foreach(var value in PropertiesByName.Values)
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
        public void Load()
        {
            EnforceDisposed();

            if(!ReferenceEquals(PropertyDef, null) && !PropertyDef.Persistence.HasFlag(ConfigurationPersistenceActions.LoadOnly))
            {
                return;
            }

            foreach(var value in PropertiesByName.Values)
            {
                if(!value.PropertyDef.Persistence.HasFlag(ConfigurationPersistenceActions.LoadOnly))
                {
                    continue;
                }

                value.ReadValue(_configurationRoot);
            }
        }

        /// <summary>
        ///     Called when a value has changed.
        /// </summary>
        /// <param name="oldValue">
        ///     The old value.
        /// </param>
        /// <param name="newValue">
        ///     The new value.
        /// </param>
        /// <exception type="System.ArgumentNullException">
        ///     <paramref name="newValue" /> was <see langword="null" />.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnValueChanged(IValue oldValue, IValue newValue)
        {
            newValue.Validate(nameof(newValue), ObjectIs.NotNull);

            OnPropertyChanged(((IPropertyValue)newValue).PropertyName);
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

            if(!ReferenceEquals(PropertyDef, null) && !PropertyDef.Persistence.HasFlag(ConfigurationPersistenceActions.SaveOnly))
            {
                return;
            }

            foreach(var value in PropertiesByName.Values)
            {
                if(!value.PropertyDef.Persistence.HasFlag(ConfigurationPersistenceActions.SaveOnly))
                {
                    continue;
                }

                value.WriteValue(_configurationRoot);
            }
        }

        /// <summary>
        ///     Sets the parent of a configuration object.
        /// </summary>
        /// <param name="parent">
        ///     The new parent object.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetParent(IConfigurationParent? parent)
        {
            _parent = parent;
        }

        /// <summary>
        ///     Clones the property values from the specified instance.
        /// </summary>
        /// <param name="value">
        ///     The instance from which to clone the values.
        /// </param>
        internal void Clone(IConfigurationObject value)
        {
            foreach(var property in PropertiesByName)
            {
                var propertyValue = property.Value.PropertyDef.PropertyInfo.GetValue(value);
                property.Value.SetValue(propertyValue);
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
                PropertiesByName.Clear();
                _propertiesByPath.Clear();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Called when a section in the configuration root has changed.
        /// </summary>
        /// <param name="sectionObject">
        ///     An object containing the section that has changed.
        /// </param>
        private void OnSectionChanged(object sectionObject)
        {
            Debug.Assert(ReferenceEquals(_configurationRoot, sectionObject));

            foreach(var section in _configurationRoot.GetChildren())
            {
                if(_propertiesByPath.TryGetValue(section.Path, out var value))
                {
                    if(value.PropertyDef.Implementation.ImplementationKind != ImplementationKind.Naive)
                    {
                        continue; //Already subscribing to changes.
                    }

                    value.ReadValue(_configurationRoot);
                }
            }

            RegisterReloadToken();
        }

        /// <summary>
        ///     Registers a reload token with the <see cref="ConfigurationRoot" />.
        /// </summary>
        private void RegisterReloadToken()
        {
            var token = _configurationRoot.GetReloadToken();
            token.RegisterChangeCallback(OnSectionChanged, _configurationRoot);
        }
    }
}