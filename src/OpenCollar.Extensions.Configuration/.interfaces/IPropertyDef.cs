﻿/*
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
using System.ComponentModel;
using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     The definition of the interface of the class that defines a property of a configuration object.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/.interfaces/IPropertyDef/IPropertyDef.svg" />
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IPropertyDef
    {
        /// <summary>
        ///     Gets or sets the default value.
        /// </summary>
        /// <value>
        ///     The default value. Can be <see langword="null" />.
        /// </value>
        object? DefaultValue
        {
            get;
        }

        /// <summary>
        ///     Gets the details of the specific implementation of this property.
        /// </summary>
        /// <value>
        ///     The details of the specific implementation of this property.
        /// </value>
        IImplementation? ElementImplementation
        {
            get;
        }

        /// <summary>
        ///     Gets a value indicating whether the property represented by this instance has default a value.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the property represented by this instance has default a value; otherwise, <see langword="false" />.
        /// </value>
        bool HasDefaultValue
        {
            get;
        }

        /// <summary>
        ///     Gets the details of the specific implementation of this property.
        /// </summary>
        /// <value>
        ///     The details of the specific implementation of this property.
        /// </value>
        IImplementation Implementation
        {
            get;
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the property is read only; otherwise, <see langword="false" /> for an
        ///     editable property.
        /// </value>
        bool IsReadOnly
        {
            get;
        }

        /// <summary>
        ///     Gets or sets the policy for when to load and save values from the configuration service.
        /// </summary>
        /// <value>
        ///     The policy for when to load and save values from the configuration service.
        /// </value>
        public ConfigurationPersistenceActions Persistence
        {
            get;
        }

        /// <summary>
        ///     Gets the property information that defines the interface property.
        /// </summary>
        /// <value>
        ///     The property information that defines the interface property.
        /// </value>
        PropertyInfo PropertyInfo
        {
            get;
        }

        /// <summary>
        ///     Gets the name of the property represented by this object.
        /// </summary>
        /// <value>
        ///     The name of the property represented by this object.
        /// </value>
        string PropertyName
        {
            get;
        }

        /// <summary>
        ///     Gets the settings used to control how configuration objects are created and the features they support.
        /// </summary>
        /// <value>
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </value>
        ConfigurationObjectSettings Settings { get; }

        /// <summary>
        ///     Gets the type of the value held in the property.
        /// </summary>
        /// <value>
        ///     The type of the value held in the property.
        /// </value>
        Type Type
        {
            get;
        }

        /// <summary>
        ///     Gets the path to this configuration object.
        /// </summary>
        /// <returns>
        ///     A string containing the path to this configuration object.
        /// </returns>
        string CalculatePath(IConfigurationParent? parent);

        /// <summary>
        ///     Parses a string value into the type defined by the property definition.
        /// </summary>
        /// <param name="path">
        ///     The path to the value being converted (used in error messages).
        /// </param>
        /// <param name="stringRepresentation">
        ///     The string to parse.
        /// </param>
        /// <returns>
        ///     The string parsed as the type of this property.
        /// </returns>
        /// <exception cref="ConfigurationException">
        ///     Value could not be converted.
        /// </exception>
        object? ConvertStringToValue(string path, string? stringRepresentation);

        /// <summary>
        ///     Given a value that can be assigned to the property represented, returns a string equivalent.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <returns>
        ///     The string equivalent of the value given.
        /// </returns>
        string? ConvertValueToString(object? value);

        /// <summary>
        ///     Copies the value of an element.
        /// </summary>
        /// <typeparam name="TElement">
        ///     The type of the element to copy.
        /// </typeparam>
        /// <param name="implementation">
        ///     The details of the implementation for which to make a copy.
        /// </param>
        /// <param name="value">
        ///     The value to copy.
        /// </param>
        /// <param name="parent">
        ///     The parent object to which cloned configuration objects will belong.
        /// </param>
        /// <param name="configurationRoot">
        ///     The configuration root from which cloned configuration objects are to be populated.
        /// </param>
        /// <returns>
        ///     The newly copied element.
        /// </returns>
        public TElement CopyValue<TElement>(IImplementation implementation, TElement value, IConfigurationParent parent, IConfigurationRoot configurationRoot);
    }
}