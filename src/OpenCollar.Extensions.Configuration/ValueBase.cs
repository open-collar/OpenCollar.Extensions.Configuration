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
using System.Diagnostics;

using Microsoft.Extensions.Configuration;

using OpenCollar.Extensions.Configuration.Resources;
using OpenCollar.Extensions.Validation;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A class used to represent a property on an interface and its location in the configuration model.
    /// </summary>
    /// <typeparam name="TParent">
    ///     The type of the parent object. Must implement <See cref="IValueChanged" /> interface.
    /// </typeparam>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/ValueBase/ValueBase.svg" />
    /// </remarks>
    /// <typeparam name="TValue">
    ///     The type of the contained value.
    /// </typeparam>
    [DebuggerDisplay("ValueBase<{typeof(TValue).Name,nq}>[{Path,nq}={StringValue}] ({CalculatePath()})")]
    internal abstract class ValueBase<TParent, TValue> : IValue, IConfigurationParent where TParent : class, IValueChanged, IConfigurationParent
    {
        /// <summary>
        ///     The parent for which this object represents a value.
        /// </summary>
        protected readonly TParent _parent;

        /// <summary>
        ///     The definition of the property represented by this value.
        /// </summary>
        protected readonly IPropertyDef _propertyDef;

        /// <summary>
        ///     A lock token used to control concurrent access to the <see cref="_currentValue" /> and
        ///     <see cref="_originalValue" /> fields.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        ///     The current (unsaved) value held in this property.
        /// </summary>
        /// <remarks>
        ///     This will be used to compare against the <see cref="_originalValue" /> to determine whether the value
        ///     has changed/
        /// </remarks>
        private TValue _currentValue;

        /// <summary>
        ///     A flag that indicates whether or not the current value has been saved.
        /// </summary>
        private bool _isSaved;

        /// <summary>
        ///     The original (saved) value of the property.
        /// </summary>
        private TValue _originalValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueBase{TParent, TValue}" /> class.
        /// </summary>
        /// <param name="propertyDef">
        ///     The definition of the property to represent.
        /// </param>
        /// <param name="parent">
        ///     The parent configuration object for which this object represents a property.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="parent" /> is <see langword="null" />.
        /// </exception>
        internal ValueBase(IPropertyDef propertyDef, TParent parent)
        {
            parent.Validate(nameof(parent), ObjectIs.NotNull);

            _parent = parent;
            _propertyDef = propertyDef;
            _isSaved = false;
        }

        /// <summary>
        ///     Gets a value indicating whether this property has unsaved changes.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if this property has unsaved changes; otherwise, <see langword="false" />.
        /// </value>
        public bool IsDirty
        {
            get
            {
                if(!_propertyDef.Persistence.HasFlag(ConfigurationPersistenceActions.SaveOnly))
                {
                    return false;
                }

                lock(_lock)
                {
                    if(!Configuration.PropertyDef.AreEqual(_originalValue, _currentValue))
                    {
                        return true;
                    }

                    if(!_isSaved && !Configuration.PropertyDef.AreEqual(_originalValue, default(TValue)))
                    {
                        return true;
                    }

                    var configObj = _currentValue as IConfigurationObject;
                    if(!ReferenceEquals(configObj, null))
                    {
                        // If the value is a configuration object then we should return its dirty state.
                        return configObj.IsDirty;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this container is read-only.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if this container is read-only; otherwise, <see langword="false" />.
        /// </value>
        public abstract bool IsReadOnly
        {
            get;
        }

        /// <summary>
        ///     Gets the parent object to which this value belongs.
        /// </summary>
        /// <value>
        ///     The parent object to which this value belongs.
        /// </value>
        public TParent Parent => _parent;

        /// <summary>
        ///     Gets the colon-delimited path to the underlying configuration value.
        /// </summary>
        /// <value>
        ///     The colon-delimited path to the underlying configuration value.
        /// </value>
        public string Path => CalculatePath();

        /// <summary>
        ///     Gets the definition of the property represented by this value.
        /// </summary>
        /// <value>
        ///     The definition of the property represented by this value.
        /// </value>
        public IPropertyDef PropertyDef => _propertyDef;

        /// <summary>
        ///     Gets or sets the value represented by this instance.
        /// </summary>
        /// <value>
        ///     The value of the property.
        /// </value>
        /// <exception cref="NotImplementedException">
        ///     This value is read-only.
        /// </exception>
        public TValue Value
        {
            get => _currentValue;

            set
            {
                if(_parent.IsReadOnly)
                {
                    throw new NotImplementedException(Exceptions.ValueIsReadOnly);
                }

                if(!SetValue(value))
                {
                    return;
                }

                _parent.OnValueChanged(this, this);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this property has unsaved changes.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if this property has unsaved changes; otherwise, <see langword="false" />.
        /// </value>
        bool IValue.IsDirty => IsDirty;

        /// <summary>
        ///     Gets the parent object to which this value belongs.
        /// </summary>
        /// <value>
        ///     The parent object to which this value belongs.
        /// </value>
        IValueChanged IValue.Parent => Parent;

        /// <summary>
        ///     Gets the colon-delimited path to the underlying configuration value.
        /// </summary>
        /// <value>
        ///     The colon-delimited path to the underlying configuration value.
        /// </value>
        string IValue.Path => Path;

        /// <summary>
        ///     Gets or sets the value represented by this instance.
        /// </summary>
        /// <value>
        ///     The value of the property.
        /// </value>
        object? IValue.Value
        {
            get => Value;
            set => Value = (TValue)value;
        }

        /// <summary>
        ///     Gets or sets the value of the property, represented as a string.
        /// </summary>
        /// <value>
        ///     The value of the property, represented as a string.
        /// </value>
        internal string? StringValue
        {
            get => _propertyDef.ConvertValueToString(_currentValue);
            set => Value = (TValue)_propertyDef.ConvertStringToValue(Path, value);
        }

        /// <summary>
        ///     Gets the implementation details of the value object.
        /// </summary>
        /// <value>
        ///     The implementation details of the value object.
        /// </value>
        protected abstract IImplementation ValueImplementation
        {
            get;
        }

        /// <summary>
        ///     Gets the full path for this value.
        /// </summary>
        /// <returns>
        ///     The full path for this value
        /// </returns>
        public abstract string CalculatePath();

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot">
        ///     The configuration root from which to read the value.
        /// </param>
        public void DeleteValue(IConfigurationRoot configurationRoot)
        {
            if(!_propertyDef.Persistence.HasFlag(ConfigurationPersistenceActions.SaveOnly))
            {
                // Configuration source is read-only.
                return;
            }

            if(_propertyDef.Implementation.ImplementationKind == ImplementationKind.Naive)
            {
                configurationRoot[CalculatePath()] = null;
            }
            else
            {
                var value = _currentValue as IConfigurationObject;
                value?.Delete();
            }
        }

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot">
        ///     The configuration root from which to read the value.
        /// </param>
        public void ReadValue(IConfigurationRoot configurationRoot)
        {
            var implementation = ValueImplementation;

            // TODO: How should we deal with values that weren't added from the source but were added by the consumer at runtime?  Flags?

            lock(_lock)
            {
                // If the value is a configuration object of some sort then create or reuse the existing value.
                IConfigurationObject? configurationObject;
                switch(implementation.ImplementationKind)
                {
                    case ImplementationKind.ConfigurationCollection:
                    case ImplementationKind.ConfigurationDictionary:
                        configurationObject = (_currentValue ?? _originalValue) as IConfigurationObject;
                        if(ReferenceEquals(configurationObject, null) || (configurationObject.GetType() != implementation.ImplementationType))
                        {
                            Value = (TValue)Activator.CreateInstance(implementation.ImplementationType, null, _propertyDef, configurationRoot, implementation.Settings);
                        }
                        else
                        {
                            Value = (TValue)configurationObject;
                        }

                        configurationObject = _currentValue as IConfigurationObject;
                        configurationObject?.Load();
                        break;

                    case ImplementationKind.ConfigurationObject:
                        configurationObject = (_currentValue ?? _originalValue) as IConfigurationObject;
                        if(ReferenceEquals(configurationObject, null) || (configurationObject.GetType() != implementation.ImplementationType))
                        {
                            Value = (TValue)Activator.CreateInstance(implementation.ImplementationType, PropertyDef, configurationRoot, null, implementation.Settings);
                        }
                        else
                        {
                            Value = (TValue)configurationObject;
                        }

                        configurationObject = _currentValue as IConfigurationObject;
                        configurationObject?.Load();
                        break;

                    default:
                        var path = CalculatePath();
                        var value = configurationRoot[path];

                        if(ReferenceEquals(value, null))
                        {
                            if(!_propertyDef.HasDefaultValue)
                            {
                                throw new ConfigurationException(path, $"No value could be found for configuration path: '{path}'.");
                            }
                            else
                            {
                                Value = (TValue)_propertyDef.DefaultValue;
                            }
                        }
                        else
                        {
                            StringValue = value;
                        }

                        break;
                }

                Saved();
            }
        }

        /// <summary>
        ///     Sets the value without firing any events.
        /// </summary>
        /// <param name="value">
        ///     The new value.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the value has changed; otherwise, <see langword="false" />.
        /// </returns>
        public bool SetValue(object? value)
        {
            try
            {
                return SetValue((TValue)value);
            }
            catch(Exception ex)
            {
                throw new ConfigurationException(_propertyDef.PropertyName, string.Format(System.Globalization.CultureInfo.InvariantCulture, Exceptions.UnableToAssignValue, _propertyDef.PropertyName), ex);
            }
        }

        /// <summary>
        ///     Writes the value to the configuration store.
        /// </summary>
        /// <param name="configurationRoot">
        ///     The configuration root to which to write the value.
        /// </param>
        public void WriteValue(IConfigurationRoot configurationRoot)
        {
            var implementation = ValueImplementation;

            lock(_lock)
            {
                switch(implementation.ImplementationKind)
                {
                    case ImplementationKind.ConfigurationCollection:
                    case ImplementationKind.ConfigurationDictionary:
                    case ImplementationKind.ConfigurationObject:
                        if(!ReferenceEquals(_originalValue, null) && ReferenceEquals(_currentValue, null))
                        {
                            // The value (and all it's sub nodes) has been deleted.
                            ((IConfigurationObject)_originalValue).Delete();
                        }
                        else
                        {
                            if(!ReferenceEquals(_currentValue, null))
                            {
                                ((IConfigurationObject)_currentValue).Save();
                            }
                            else
                            {
                                // TODO: Make sure all mention of the value has been deleted.
                            }
                        }

                        break;

                    default:
                        configurationRoot[CalculatePath()] = StringValue;
                        break;
                }

                Saved();
            }
        }

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot">
        ///     The configuration root from which to read the value.
        /// </param>
        void IValue.DeleteValue(IConfigurationRoot configurationRoot) => DeleteValue(configurationRoot);

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot">
        ///     The configuration root from which to read the value.
        /// </param>
        void IValue.ReadValue(IConfigurationRoot configurationRoot) => ReadValue(configurationRoot);

        /// <summary>
        ///     Writes the value to the configuration store.
        /// </summary>
        /// <param name="configurationRoot">
        ///     The configuration root to which to write the value.
        /// </param>
        void IValue.WriteValue(IConfigurationRoot configurationRoot) => WriteValue(configurationRoot);

        /// <summary>
        ///     Called when the current value has been saved to he underlying configuration source.
        /// </summary>
        internal void Saved()
        {
            lock(_lock)
            {
                _isSaved = true;
                _originalValue = _currentValue;
            }
        }

        /// <summary>
        ///     Sets the value without firing any events.
        /// </summary>
        /// <param name="value">
        ///     The new value.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the value has changed; otherwise, <see langword="false" />.
        /// </returns>
        internal bool SetValue(TValue value)
        {
            lock(_lock)
            {
                if(Configuration.PropertyDef.AreEqual(_originalValue, value))
                {
                    return false;
                }

                _currentValue = value;

                var child = value as IConfigurationChild;
                child?.SetParent(this);

                return true;
            }
        }
    }
}