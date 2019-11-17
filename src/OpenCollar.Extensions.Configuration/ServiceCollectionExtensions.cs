
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// Extensions to the <see cref="IServiceCollection"/> type allowing configuration objects to be registered.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add a new kind of configuration reader that represents values taken directly from the <see cref="Microsoft.Extensions.Configuration.IConfigurationRoot"/> object in the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection to which to add the configuration reader.  This must not be <see langword="null"/>.</param>
        /// <typeparam name="TConfigurationObject">The interface through which consumers will access the configuration.
        /// This must be derived from the <see cref="IConfigurationObject"/> interface.</typeparam>
        /// <exception type="System.ArgumentNullException"><paramref name="serviceCollection"/> was <see langword="null"/>.</exception>
        public static void AddConfigurationReader<TConfigurationObject>(this IServiceCollection serviceCollection) where TConfigurationObject : IConfigurationObject
        {
            // Check to see if the collection has the relevant configuration reader registered, and if not, create and add a new instance.

            var serviceType = typeof(TConfigurationObject);

            if (serviceCollection.Any(d => d.ServiceType == serviceType))
            {
                return;
            }

            var context = new ConfigurationContext();

            var implementationType = GenerateConfigurationObjectType<TConfigurationObject>(context);

            var descriptor = new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Singleton);

            serviceCollection.Add(descriptor);
        }

        /// <summary>Creates the type of the configuration object.</summary>
        /// <typeparam name="TConfigurationObject">The type of the interface to be implemented by the configuration object to create.</typeparam>
        /// <param name="context">Defines the context in which the configuration is to be constructed.</param>
        /// <returns>
        /// An implementation of the interface specified that can be used to interact with the configuration.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Type specifies more than one 'Path' attribute and so cannot be processed.
        /// - or -
        /// Property specifies more than one 'Path' attribute and so cannot be processed.
        /// </exception>
        private static Type GenerateConfigurationObjectType<TConfigurationObject>(ConfigurationContext context) where TConfigurationObject : IConfigurationObject
        {
            var type = typeof(TConfigurationObject);
            var localContext = new ConfigurationContext(context);

            var pathAttributes = type.GetCustomAttributes(typeof(PathAttribute), true);
            if (!ReferenceEquals(pathAttributes, null) && (pathAttributes.Length > 0))
            {
                if (pathAttributes.Length > 1)
                {
                    throw new InvalidOperationException($"Type '{type.Namespace}.{type.Name}' specifies more than one 'Path' attribute and so cannot be processed.");
                }
                localContext.ApplyPathAttribute((PathAttribute)pathAttributes[0]);
            }

            var propertyDefs = new System.Collections.Generic.List<PropertyDef>();

            foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (!property.CanRead)
                {
                    continue;
                }

                var path = localContext.Path;
                var name = property.Name;

                pathAttributes = property.GetCustomAttributes(typeof(PathAttribute), true);
                if (!ReferenceEquals(pathAttributes, null) && (pathAttributes.Length > 0))
                {
                    if (pathAttributes.Length > 1)
                    {
                        throw new InvalidOperationException($"Property '{type.Namespace}.{type.Name}.{name}' specifies more than one 'Path' attribute and so cannot be processed.");
                    }
                    var pathAttribute = ((PathAttribute)pathAttributes[0]);

                    switch (pathAttribute.Usage)
                    {
                        case PathIs.Root:
                            path = pathAttribute.Path;
                            break;

                        case PathIs.Suffix:
                            if (string.IsNullOrWhiteSpace(path))
                            {
                                path = pathAttribute.Path;
                            }
                            else
                            {
                                path = string.Concat(path, ConfigurationContext.PathDelimiter, pathAttribute.Path);
                            }
                            break;
                    }
                }

                if (typeof(IConfigurationObject).IsAssignableFrom(property.PropertyType))
                {
                    // The property represents another level in the tree.
                }

                propertyDefs.Add(new PropertyDef(path, name, property.PropertyType, !property.CanWrite));
            }

            return typeof(TConfigurationObject);
        }
    }
}