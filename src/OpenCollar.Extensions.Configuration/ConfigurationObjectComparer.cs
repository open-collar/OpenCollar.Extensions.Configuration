using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A comparer for objects that implement interfaces derived from <see cref="IConfigurationObject" />.
    /// </summary>
    /// <seealso cref="IEqualityComparer{T}" />
    public class ConfigurationObjectComparer : IEqualityComparer<IConfigurationObject>
    {
        /// <summary>
        ///     Prevents a default instance of the <see cref="ConfigurationObjectComparer" /> class from being created.
        /// </summary>
        private ConfigurationObjectComparer()
        {
        }

        /// <summary>
        ///     Gets an instance of the comparer
        /// </summary>
        /// <value> The instance of the comparer </value>
        public static ConfigurationObjectComparer Instance { get; } = new ConfigurationObjectComparer();

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x"> The first object of type <see name="IConfigurationObject" /> to compare. </param>
        /// <param name="y"> The second object of type <see name="IConfigurationObject" /> to compare. </param>
        /// <returns> <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />. </returns>
        public bool Equals(IConfigurationObject x, IConfigurationObject y)
        {
            if(ReferenceEquals(x, y))
            {
                return true;
            }

            if(ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            List<PropertyInfo> properties;

            if(x.GetType() == y.GetType())
            {
                properties = GetPropertyInfo(x.GetType()).ToList();
            }
            else
            {
                properties = new List<PropertyInfo>();
                var xInterfaces = GetConfigurationObjectInterfaces(x.GetType()).ToArray();
                var yInterfaces = GetConfigurationObjectInterfaces(y.GetType()).ToArray();
                if(xInterfaces.Length != yInterfaces.Length)
                {
                    return false;
                }

                if(xInterfaces.Union(yInterfaces).Count() != xInterfaces.Length)
                {
                    return false;
                }

                foreach(var interfaceType in xInterfaces)
                {
                    properties.AddRange(GetPropertyInfo(interfaceType));
                }

                properties = properties.Distinct().ToList();
            }

            foreach(var property in properties)
            {
                if(!Equals(property.GetValue(x), property.GetValue(y)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj"> The object. </param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(IConfigurationObject obj)
        {
            var properties = GetConfigurationObjectProperties(obj.GetType());

            var hashcode = 1;

            unchecked
            {
                foreach(var property in properties)
                {
                    var value = property.GetValue(obj);
                    if(!ReferenceEquals(value, null))
                    {
                        hashcode = hashcode * value.GetHashCode();
                    }
                }
            }

            return hashcode;
        }

        /// <summary>
        ///     Gets the interfaces derived from <see cref="IConfigurationObject" />.
        /// </summary>
        /// <param name="type"> The type to analyze. </param>
        /// <returns> Returns a sequence of types representing the interfaces. </returns>
        private static IEnumerable<Type> GetConfigurationObjectInterfaces(Type type)
        {
            foreach(var interfaceType in type.GetInterfaces())
            {
                if(typeof(IConfigurationObject).IsAssignableFrom(interfaceType))
                {
                    if(interfaceType == typeof(IConfigurationObject))
                    {
                        continue;
                    }

                    yield return interfaceType;
                }
            }
        }

        /// <summary>
        ///     Gets a distinct list of the definitions of properties defined in interfaces derived from
        ///     <see cref="IConfigurationObject" /> by the type given.
        /// </summary>
        /// <param name="type"> The type for which to retrieve the properties. </param>
        /// <returns>
        ///     The distinct list of the definitions of properties defined in interfaces derived from
        ///     <see cref="IConfigurationObject" /> by the type given.
        /// </returns>
        private static List<PropertyInfo> GetConfigurationObjectProperties(Type type)
        {
            var properties = new List<PropertyInfo>();

            foreach(var interfaceType in GetConfigurationObjectInterfaces(type))
            {
                properties.AddRange(GetPropertyInfo(interfaceType));
            }

            properties = properties.Distinct().ToList();

            return properties;
        }

        /// <summary>
        ///     Gets the relevant properties from the type given.
        /// </summary>
        /// <param name="type"> The type for which to return the properties. </param>
        /// <returns> Returns a sequence of the relevant properties. </returns>
        private static IEnumerable<PropertyInfo> GetPropertyInfo(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead);
        }
    }
}