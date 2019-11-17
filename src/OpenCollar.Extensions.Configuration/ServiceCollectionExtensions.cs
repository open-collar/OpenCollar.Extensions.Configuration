
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

            var implementationType = GenerateConfigurationObjectType<TConfigurationObject>();

            var descriptor = new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Singleton);

            serviceCollection.Add(descriptor);
        }

        /// <summary>Creates the type of the configuration object.</summary>
        /// <typeparam name="TConfigurationObject">The type of the interface to be implemented by the configuration object to create.</typeparam>
        /// <returns>The a type that implemnents the interface required.</returns>
        private static Type GenerateConfigurationObjectType<TConfigurationObject>() where TConfigurationObject : IConfigurationObject
        {
            return typeof(string);
        }
    }
}