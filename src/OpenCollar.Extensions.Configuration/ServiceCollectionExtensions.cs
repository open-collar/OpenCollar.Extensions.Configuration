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
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenCollar.Extensions.Configuration.Validation;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Extensions to the <see cref="IServiceCollection" /> type allowing configuration objects to be registered.
    /// </summary>
    /// <example>
    ///     <para>
    ///         The starting point is to define an interface through which to read your configuration. The interface
    ///         must derive from <see cref="IConfigurationObject"/>. The interfaces must be public. See th example below.
    ///     </para>
    ///     <code lang="cs">
    ///public interface IEnvironment : IConfigurationObject
    ///{
    ///    public string EnvironmentName { get; }
    ///
    ///    public string Version { get; }
    ///}
    ///
    ///public interface IMyConfig : IConfigurationObject
    ///{
    ///    public IEnvironment Environment { get; }
    ///
    ///    public string ReadOnlyString { get; }
    ///
    ///    public string ReadWriteString { get; }
    ///}
    ///     </code>
    ///     <para>
    ///         The next step is to register the interface as a service in
    ///         <see href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-3.1">
    ///         Startup.cs </see>. At the same time the
    ///         <see href="https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationroot?view=dotnet-plat-ext-3.1">
    ///         IConfigurationRoot </see> object for the application must also be registered as a service.
    ///     </para>
    ///     <code lang="cs">
    ///public class Startup
    ///{
    ///    private readonly IConfigurationRoot _configuration;
    ///
    ///    public Startup(IConfiguration configuration)
    ///    {
    ///        // Capture the configuration object passed in when the application is started.
    ///        _configuration = (IConfigurationRoot)configuration;
    ///    }
    ///
    ///    public void ConfigureServices(IServiceCollection services)
    ///    {
    ///        services.AddRazorPages();
    ///        services.AddSingleton(_configuration);
    ///        services.AddConfigurationReader&lt;IMyConfig&gt;();
    ///    }
    ///...
    ///}
    ///     </code>
    ///     <para> Later, when needed, the configuration reader is available as a service: </para>
    ///     <code lang="cs">
    ///public MyConstructor(IMyConfig config)
    ///{
    ///    var version = config.Environment.Version;
    ///}
    ///     </code>
    /// </example>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/ServiceCollectionExtensions/ServiceCollectionExtensions.svg" />
    /// </remarks>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Add a new kind of configuration reader that represents values taken directly from the
        ///     <see cref="IConfigurationRoot" /> object in the service collection.
        /// </summary>
        /// <typeparam name="TConfigurationObject">
        ///     The interface through which consumers will access the configuration. This must be derived from the
        ///     <see cref="IConfigurationObject" /> interface.
        /// </typeparam>
        /// <param name="serviceCollection">
        ///     The service collection to which to add the configuration reader. This must not be <see langword="null" />.
        /// </param>
        public static IServiceCollection AddConfigurationReader<TConfigurationObject>(this IServiceCollection serviceCollection)
        where TConfigurationObject : IConfigurationObject
        {
            return AddConfigurationReader<TConfigurationObject>(serviceCollection, null);
        }

        /// <summary>
        ///     Add a new kind of configuration reader that represents values taken directly from the
        ///     <see cref="IConfigurationRoot" /> object in the service collection.
        /// </summary>
        /// <typeparam name="TConfigurationObject">
        ///     The interface through which consumers will access the configuration. This must be derived from the
        ///     <see cref="IConfigurationObject" /> interface.
        /// </typeparam>
        /// <param name="serviceCollection">
        ///     The service collection to which to add the configuration reader. This must not be <see langword="null" />.
        /// </param>
        /// <param name="settings">
        ///     Optional settings used to control how configuration objects are created and the features they support.
        /// </param>
        public static IServiceCollection AddConfigurationReader<TConfigurationObject>(this IServiceCollection serviceCollection, ConfigurationObjectSettings? settings = null)
    where TConfigurationObject : IConfigurationObject
        {
            serviceCollection.Validate(nameof(serviceCollection), ObjectIs.NotNull);

            // Check to see if the collection has the relevant configuration reader registered, and if not, create and
            // add a new instance.

            if(ReferenceEquals(settings, null))
            {
                settings = new ConfigurationObjectSettings();
            }

            var serviceType = typeof(TConfigurationObject);

            if(serviceCollection.Any(d => d.ServiceType == serviceType))
            {
                return serviceCollection;
            }

            var implementationType = GenerateConfigurationObjectType<TConfigurationObject>(settings);

            var descriptor = new ServiceDescriptor(serviceType, (provider) =>
            {
                var configurationRoot = provider.GetService<IConfigurationRoot>();

                var configurationObject = (TConfigurationObject)Activator.CreateInstance(implementationType, null, configurationRoot, null, settings);

                configurationObject.Load();

                return configurationObject;
            }, ServiceLifetime.Singleton);

            serviceCollection.Add(descriptor);

            return serviceCollection;
        }

        /// <summary>
        ///     Creates the type of the configuration object.
        /// </summary>
        /// <param name="settings">
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </param>
        /// <typeparam name="TConfigurationObject">
        ///     The type of the interface to be implemented by the configuration object to create.
        /// </typeparam>
        /// <returns>
        ///     An implementation of the interface specified that can be used to interact with the configuration.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Type specifies more than one 'Path' attribute and so cannot be processed. - or - Property specifies more
        ///     than one 'Path' attribute and so cannot be processed.
        /// </exception>
        internal static Type GenerateConfigurationObjectType<TConfigurationObject>(ConfigurationObjectSettings settings) where TConfigurationObject : IConfigurationObject
        {
            var type = typeof(TConfigurationObject);

            return GenerateConfigurationObjectType(type, settings);
        }

        /// <summary>
        ///     Creates the type of the configuration object.
        /// </summary>
        /// <param name="type">
        ///     The type of the interface to be implemented by the configuration object to create.
        /// </param>
        /// <param name="settings">
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </param>
        /// <returns>
        ///     An implementation of the interface specified that can be used to interact with the configuration.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Type specifies more than one 'Path' attribute and so cannot be processed. - or - Property specifies more
        ///     than one 'Path' attribute and so cannot be processed.
        /// </exception>
        internal static Type GenerateConfigurationObjectType(Type type, ConfigurationObjectSettings settings)
        {
            var propertyDefs = GetConfigurationObjectDefinition(type, settings);

            var builder = new ConfigurationObjectTypeBuilder(type, propertyDefs, settings);

            var generatedType = builder.Generate();

            return generatedType;
        }

        /// <summary>
        ///     Gets the configuration object definition.
        /// </summary>
        /// <param name="type">
        ///     The type of the object to define.
        /// </param>
        /// <param name="settings">
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </param>
        /// <returns>
        ///     A list of the property definitions for the type specified.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Type '{type.Namespace}.{type.Name}' specifies more than one 'Path' attribute and so cannot be processed.
        /// </exception>
        internal static List<PropertyDef> GetConfigurationObjectDefinition(Type type, ConfigurationObjectSettings settings)
        {
            // TODO: Circular reference detection - for this version.

            var propertyDefs = new List<PropertyDef>();

            foreach(var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if(!property.CanRead)
                {
                    continue;
                }

                var propertyDef = new PropertyDef(property, settings);

                propertyDefs.Add(propertyDef);
            }

            return propertyDefs;
        }
    }
}