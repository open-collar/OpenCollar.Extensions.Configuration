using System;
using System.Diagnostics;
using System.Linq;

using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     The details of the implementation of a property or element.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class Implementation : IImplementation
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Implementation" /> class.
        /// </summary>
        /// <param name="underlyingType"> The underlying type of the property or collection represented. </param>
        /// <param name="isReadOnly">
        ///     If set to <see langword="true" /> the property or collection is treated as a read-only.
        /// </param>
        internal Implementation(Type underlyingType, bool isReadOnly)
        {
            ImplementationKind = GetImplementationKind(underlyingType);

            Type elementType;

            switch(ImplementationKind)
            {
                case ImplementationKind.ConfigurationObject:
                    ImplementationType = ServiceCollectionExtensions.GenerateConfigurationObjectType(underlyingType);
                    Type = underlyingType;
                    break;

                case ImplementationKind.ConfigurationCollection:
                    elementType = underlyingType.GenericTypeArguments.First();
                    if(isReadOnly)
                    {
                        ImplementationType = typeof(ReadOnlyConfigurationCollection<>).MakeGenericType(new[] { elementType });
                    }
                    else
                    {
                        ImplementationType = typeof(ConfigurationCollection<>).MakeGenericType(new[] { elementType });
                    }

                    Type = elementType;
                    break;

                case ImplementationKind.ConfigurationDictionary:
                    elementType = underlyingType.GenericTypeArguments.First();
                    if(isReadOnly)
                    {
                        ImplementationType = typeof(ReadOnlyConfigurationDictionary<>).MakeGenericType(new[] { elementType });
                    }
                    else
                    {
                        ImplementationType = typeof(ConfigurationDictionary<>).MakeGenericType(new[] { elementType });
                    }

                    Type = elementType;
                    break;

                default:
                    ImplementationType = null;
                    Type = underlyingType;
                    break;
            }
        }

        /// <summary>
        ///     Gets the kind of the implementation to use to instantiate values.
        /// </summary>
        /// <value> The kind of the implementation to use to instantiate values. </value>
        public ImplementationKind ImplementationKind
        {
            get;
        }

        /// <summary>
        ///     Gets the type of the object that implements values ( <see langword="null" /> if the property is naive).
        /// </summary>
        /// <value>
        ///     The type of the object that implements values ( <see langword="null" /> if the property is naive).
        /// </value>
        public Type? ImplementationType
        {
            get;
        }

        /// <summary>
        ///     Gets the type of the value represented (the type of the property).
        /// </summary>
        /// <value> The type of the value represented (the type of the property). </value>
        public Type Type
        {
            get;
        }

        /// <summary>
        ///     Gets the kind of the implementation required for the type given.
        /// </summary>
        /// <param name="type"> The type for which the implementation kind is required. </param>
        /// <returns> </returns>
        private static ImplementationKind GetImplementationKind(Type type)
        {
            if(IsConfigurationDictionary(type))
            {
                return ImplementationKind.ConfigurationDictionary;
            }

            if(IsConfigurationCollection(type))
            {
                return ImplementationKind.ConfigurationCollection;
            }

            if(typeof(IConfigurationObject).IsAssignableFrom(type))
            {
                return ImplementationKind.ConfigurationObject;
            }

            return ImplementationKind.Naive;
        }

        /// <summary>
        ///     Determines whether the specified type is a is configuration collection.
        /// </summary>
        /// <param name="type"> The type to verify. </param>
        /// <returns> <see langword="true" /> if the type is configuration collection; otherwise, <see langword="false" />. </returns>
        private static bool IsConfigurationCollection(Type type)
        {
            if(!type.IsConstructedGenericType)
            {
                return false;
            }

            if(type.GetGenericTypeDefinition() != typeof(IConfigurationCollection<>))
            {
                return false;
            }

            var arguments = type.GetGenericArguments();

            Debug.Assert(arguments.Length == 1);

            return typeof(IConfigurationObject).IsAssignableFrom(arguments[0]);
        }

        /// <summary>
        ///     Determines whether the specified type is a is configuration dictionary.
        /// </summary>
        /// <param name="type"> The type to verify. </param>
        /// <returns> <see langword="true" /> if the type is configuration dictionary; otherwise, <see langword="false" />. </returns>
        private static bool IsConfigurationDictionary(Type type)
        {
            if(!type.IsConstructedGenericType)
            {
                return false;
            }

            if(type.GetGenericTypeDefinition() != typeof(IConfigurationDictionary<>))
            {
                return false;
            }

            var arguments = type.GetGenericArguments();

            Debug.Assert(arguments.Length == 1);

            return typeof(IConfigurationObject).IsAssignableFrom(arguments[0]);
        }
    }
}