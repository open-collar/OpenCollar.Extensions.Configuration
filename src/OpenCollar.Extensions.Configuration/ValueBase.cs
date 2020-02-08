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

using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A class used to represent a property on an interface and its location in the configuration model.
    /// </summary>
    /// <typeparam name="TParent">
    ///     The type of the parent object. Must implement <See cref="IValueChanged" /> interface.
    /// </typeparam>
    /// <typeparam name="TValue"> The type of the contained value. </typeparam>
    [System.Diagnostics.DebuggerDisplay("{Path,nq}=\"{StringValue}\"")]
    public abstract class ValueBase<TParent, TValue> : IValue where TParent : IValueChanged, IConfigurationParent
    {
        /// <summary>
        ///     The parent for which this object represents a value.
        /// </summary>
        protected readonly TParent _parent;

        /// <summary>
        ///     The definition of the property represented by this value.
        /// </summary>
        protected readonly PropertyDef _propertyDef;

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
        /// <param name="propertyDef"> The definition of the property to represent. </param>
        /// <param name="parent"> The parent configuration object for which this object represents a property. </param>
        /// <param name="value"> The initial value to assign. </param>
        internal ValueBase(PropertyDef propertyDef, TParent parent, TValue value)
        {
            _parent = parent;
            _propertyDef = propertyDef;
            _currentValue = value;
            _originalValue = value;
            _isSaved = false;
        }

        /// <summary>
        ///     Gets a value indicating whether this property has unsaved changes.
        /// </summary>
        /// <value> <see langword="true" /> if this property has unsaved changes; otherwise, <see langword="false" />. </value>
        public bool IsDirty
        {
            get
            {
                lock(_lock)
                {
                    if(!_propertyDef.AreEqual(_originalValue, _currentValue))
                    {
                        return true;
                    }

                    if(!_isSaved && !_propertyDef.AreEqual(_originalValue, default(TValue)))
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
        ///     Gets the parent object to which this value belongs.
        /// </summary>
        /// <value> The parent object to which this value belongs. </value>
        public TParent Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        ///     Gets the colon-delimited path to the underlying configuration value.
        /// </summary>
        /// <value> The colon-delimited path to the underlying configuration value. </value>
        public string Path
        {
            get
            {
                return GetPath();
            }
        }

        /// <summary>
        ///     Gets the definition of the property represented by this value.
        /// </summary>
        /// <value> The definition of the property represented by this value. </value>
        public PropertyDef PropertyDef
        {
            get
            {
                return _propertyDef;
            }
        }

        /// <summary>
        ///     Gets or sets the value represented by this instance.
        /// </summary>
        /// <value> The value of the property. </value>
        /// <exception cref="NotImplementedException"> This value is read-only. </exception>
        public TValue Value
        {
            get => _currentValue;
            set
            {
                if(_parent.IsReadOnly)
                {
                    throw new NotImplementedException("This value is read-only.");
                }

                lock(_lock)
                {
                    if(_propertyDef.AreEqual(_originalValue, value))
                    {
                        return;
                    }

                    _currentValue = value;
                }

                _parent.OnValueChanged(this, this);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this property has unsaved changes.
        /// </summary>
        /// <value> <see langword="true" /> if this property has unsaved changes; otherwise, <see langword="false" />. </value>
        bool IValue.IsDirty
        {
            get
            {
                return IsDirty;
            }
        }

        /// <summary>
        ///     Gets the parent object to which this value belongs.
        /// </summary>
        /// <value> The parent object to which this value belongs. </value>
        IValueChanged IValue.Parent
        {
            get
            {
                return Parent;
            }
        }

        /// <summary>
        ///     Gets the colon-delimited path to the underlying configuration value.
        /// </summary>
        /// <value> The colon-delimited path to the underlying configuration value. </value>
        string IValue.Path
        {
            get
            {
                return Path;
            }
        }

        /// <summary>
        ///     Gets or sets the value represented by this instance.
        /// </summary>
        /// <value> The value of the property. </value>
        object? IValue.Value
        {
            get
            {
                return Value;
            }
            set
            {
                Value = (TValue)value;
            }
        }

        /// <summary>
        ///     Gets or sets the value of the property, represented as a string.
        /// </summary>
        /// <value> The value of the property, represented as a string. </value>
        internal string? StringValue
        {
            get
            {
                return _propertyDef.ConvertValueToString(_currentValue);
            }
            set
            {
                Value = (TValue)_propertyDef.ConvertStringToValue(Path, value);
            }
        }

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read the value. </param>
        public void DeleteValue(IConfigurationRoot configurationRoot)
        {
            if(_propertyDef.Implementation.ImplementationKind == ImplementationKind.Naive)
            {
                configurationRoot[GetPath()] = null;
            }
            else
            {
                var value = _currentValue as IConfigurationObject;
                if(!ReferenceEquals(value, null))
                {
                    value.Delete();
                }
            }
        }

        /// <summary>
        ///     Gets the full path for this value.
        /// </summary>
        /// <returns> The full path for this value </returns>
        public abstract string GetPath();

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read the value. </param>
        public void ReadValue(IConfigurationRoot configurationRoot)
        {
            lock(_lock)
            {
                // If the value is a configuration object of some sort then create or reuse the existing value;
                IConfigurationObject? configurationObject;
                switch(_propertyDef.Implementation.ImplementationKind)
                {
                    case ImplementationKind.ConfigurationCollection:
                    case ImplementationKind.ConfigurationDictionary:
                        configurationObject = (_currentValue ?? _originalValue) as IConfigurationObject;
                        if(ReferenceEquals(configurationObject, null) || (configurationObject.GetType() != _propertyDef.Implementation.ImplementationType))
                        {
                            Value = (TValue)Activator.CreateInstance(_propertyDef.Implementation.ImplementationType, (IConfigurationParent)this, _propertyDef, configurationRoot);
                        }
                        else
                        {
                            configurationObject.Reload();
                            Value = (TValue)configurationObject;
                        }
                        break;

                    case ImplementationKind.ConfigurationObject:
                        configurationObject = (_currentValue ?? _originalValue) as IConfigurationObject;
                        if(ReferenceEquals(configurationObject, null) || (configurationObject.GetType() != _propertyDef.Implementation.ImplementationType))
                        {
                            Value = (TValue)Activator.CreateInstance(_propertyDef.Implementation.ImplementationType, _propertyDef, (IConfigurationParent)this, configurationRoot, true);
                        }
                        else
                        {
                            configurationObject.Reload();
                            Value = (TValue)configurationObject;
                        }
                        break;

                    default:
                        var path = GetPath();
                        var value = configurationRoot[path];

                        if(ReferenceEquals(value, null) && !_propertyDef.IsNullable)
                        {
                            throw new ConfigurationException(path, $"No value could be found for configuration path: '{path}'.");
                        }

                        if(ReferenceEquals(value, null) && _propertyDef.IsNullable)
                        {
                            Value = (TValue)_propertyDef.DefaultValue;
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
        ///     Writes the value to the configuration store.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root to which to write the value. </param>
        public void WriteValue(IConfigurationRoot configurationRoot)
        {
            lock(_lock)
            {
                switch(_propertyDef.Implementation.ImplementationKind)
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
                        configurationRoot[GetPath()] = StringValue;
                        break;
                }
                Saved();
            }
        }

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read the value. </param>
        void IValue.DeleteValue(IConfigurationRoot configurationRoot) => DeleteValue(configurationRoot);

        /// <summary>
        ///     Reads the value of the value identified by <see cref="PropertyDef" /> from the configuration root given.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root from which to read the value. </param>
        void IValue.ReadValue(IConfigurationRoot configurationRoot) => ReadValue(configurationRoot);

        /// <summary>
        ///     Writes the value to the configuration store.
        /// </summary>
        /// <param name="configurationRoot"> The configuration root to which to write the value. </param>
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
    }
}