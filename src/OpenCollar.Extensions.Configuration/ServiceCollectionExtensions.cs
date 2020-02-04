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
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Extensions to the <see cref="IServiceCollection" /> type allowing configuration objects to be registered.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     A thread-safe dictionary of the property definition collections keyed on the type of the class that
        ///     defines them.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyDef> _propertyDefs = new ConcurrentDictionary<Type, PropertyDef>();

        /// <summary>
        ///     Add a new kind of configuration reader that represents values taken directly from the
        ///     <see cref="Microsoft.Extensions.Configuration.IConfigurationRoot" /> object in the service collection.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection to which to add the configuration reader. This must not be <see langword="null" />.
        /// </param>
        /// <typeparam name="TConfigurationObject">
        ///     The interface through which consumers will access the configuration. This must be derived from the
        ///     <see cref="IConfigurationObject" /> interface.
        /// </typeparam>
        /// <exception type="System.ArgumentNullException"> <paramref name="serviceCollection" /> was <see langword="null" />. </exception>
        public static void AddConfigurationReader<TConfigurationObject>(this IServiceCollection serviceCollection)
            where TConfigurationObject : IConfigurationObject
        {
            // Check to see if the collection has the relevant configuration reader registered, and if not, create and
            // add a new instance.

            var serviceType = typeof(TConfigurationObject);

            if(serviceCollection.Any(d => d.ServiceType == serviceType))
            {
                return;
            }

            var implementationType = GenerateConfigurationObjectType<TConfigurationObject>();

            var descriptor = new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Singleton);

            serviceCollection.Add(descriptor);
        }

        /// <summary>
        ///     Creates the type of the configuration object.
        /// </summary>
        /// <typeparam name="TConfigurationObject">
        ///     The type of the interface to be implemented by the configuration object to create.
        /// </typeparam>
        /// <returns> An implementation of the interface specified that can be used to interact with the configuration. </returns>
        /// <exception cref="InvalidOperationException">
        ///     Type specifies more than one 'Path' attribute and so cannot be processed. - or - Property specifies more
        ///     than one 'Path' attribute and so cannot be processed.
        /// </exception>
        internal static Type GenerateConfigurationObjectType<TConfigurationObject>()
            where TConfigurationObject : IConfigurationObject
        {
            var type = typeof(TConfigurationObject);

            return GenerateConfigurationObjectType(type);
        }

        /// <summary>
        ///     Creates the type of the configuration object.
        /// </summary>
        /// <param name="type"> The type of the interface to be implemented by the configuration object to create. </param>
        /// <param name="context"> Defines the context in which the configuration is to be constructed. </param>
        /// <returns> An implementation of the interface specified that can be used to interact with the configuration. </returns>
        /// <exception cref="InvalidOperationException">
        ///     Type specifies more than one 'Path' attribute and so cannot be processed. - or - Property specifies more
        ///     than one 'Path' attribute and so cannot be processed.
        /// </exception>
        internal static Type GenerateConfigurationObjectType(Type type)
        {
            var propertyDefs = GetConfigurationObjectDefinition(type);

            var builder = new ConfigurationObjectTypeBuilder(type, propertyDefs);

            return builder.Generate();
        }

        /// <summary>
        ///     Gets the configuration object definition.
        /// </summary>
        /// <param name="type"> The type of the object to define. </param>
        /// <returns> A list of the property definitions for the type specified. </returns>
        /// <exception cref="InvalidOperationException">
        ///     Type '{type.Namespace}.{type.Name}' specifies more than one 'Path' attribute and so cannot be processed.
        /// </exception>
        internal static List<PropertyDef> GetConfigurationObjectDefinition(Type type)
        {
            // TODO: Circular reference detection - for this version.

            var pathAttributes = type.GetCustomAttributes(typeof(PathAttribute), true);
            if(!ReferenceEquals(pathAttributes, null) && (pathAttributes.Length > 0))
            {
                if(pathAttributes.Length > 1)
                {
                    throw new InvalidOperationException(
                        $"Type '{type.Namespace}.{type.Name}' specifies more than one 'Path' attribute and so cannot be processed.");
                }
            }

            var propertyDefs = new List<PropertyDef>();

            foreach(var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if(!property.CanRead)
                {
                    continue;
                }

                var defaultValueAttributes = property.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                if(ReferenceEquals(defaultValueAttributes, null) || (defaultValueAttributes.Length <= 0))
                {
                    propertyDefs.Add(new PropertyDef(type, property));
                }
                else
                {
                    var defaultValue = ((DefaultValueAttribute)defaultValueAttributes[0]).DefaultValue;
                    propertyDefs.Add(new PropertyDef(type, property, defaultValue));
                }
            }

            return propertyDefs;
        }
    }
}