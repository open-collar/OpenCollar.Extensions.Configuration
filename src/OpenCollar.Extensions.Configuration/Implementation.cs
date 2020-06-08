using System;
using System.ComponentModel;
using System.Linq;

using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     The details of the implementation of a property or element.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/Implementation/Implementation.svg" />
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class Implementation : IImplementation
    {
        /// <summary>
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </summary>
        private readonly ConfigurationObjectSettings _settings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Implementation" /> class.
        /// </summary>
        /// <param name="underlyingType">
        ///     The underlying type of the property or collection represented.
        /// </param>
        /// <param name="settings">
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </param>
        internal Implementation(Type underlyingType, ConfigurationObjectSettings settings)
        {
            ImplementationKind = GetImplementationKind(underlyingType, out var isReadOnlyColectionType);
            _settings = settings;

            Type elementType;

            switch(ImplementationKind)
            {
                case ImplementationKind.ConfigurationObject:
                    ImplementationType = ServiceCollectionExtensions.GenerateConfigurationObjectType(underlyingType, settings);
                    Type = underlyingType;
                    break;

                case ImplementationKind.ConfigurationCollection:
                    elementType = underlyingType.GenericTypeArguments.First();
                    if(isReadOnlyColectionType)
                    {
                        ImplementationType = typeof(ReadOnlyConfigurationCollection<>).MakeGenericType(elementType);
                    }
                    else
                    {
                        ImplementationType = typeof(ConfigurationCollection<>).MakeGenericType(elementType);
                    }

                    Type = elementType;
                    break;

                case ImplementationKind.ConfigurationDictionary:
                    elementType = underlyingType.GenericTypeArguments.First();
                    if(isReadOnlyColectionType)
                    {
                        ImplementationType = typeof(ReadOnlyConfigurationDictionary<>).MakeGenericType(elementType);
                    }
                    else
                    {
                        ImplementationType = typeof(ConfigurationDictionary<>).MakeGenericType(elementType);
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
        /// <value>
        ///     The kind of the implementation to use to instantiate values.
        /// </value>
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
        ///     Gets the settings used to control how configuration objects are created and the features they support.
        /// </summary>
        /// <value>
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </value>
        public ConfigurationObjectSettings Settings { get { return _settings; } }

        /// <summary>
        ///     Gets the type of the value represented (the type of the property).
        /// </summary>
        /// <value>
        ///     The type of the value represented (the type of the property).
        /// </value>
        public Type Type
        {
            get;
        }

        /// <summary>
        ///     Gets the kind of the implementation required for the type given.
        /// </summary>
        /// <param name="type">
        ///     The type for which the implementation kind is required.
        /// </param>
        /// <param name="isReadOnly">
        ///     Returns a value which, if set to <see langword="true" />, indicates that the implementation is read-only.
        /// </param>
        /// <returns>
        /// </returns>
        private static ImplementationKind GetImplementationKind(Type type, out bool isReadOnly)
        {
            if(type.IsConstructedGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if(genericType == typeof(IReadOnlyConfigurationDictionary<>))
                {
                    if(type.GetGenericArguments().Length == 1)
                    {
                        isReadOnly = true;
                        return ImplementationKind.ConfigurationDictionary;
                    }
                }

                if(genericType == typeof(IConfigurationDictionary<>))
                {
                    if(type.GetGenericArguments().Length == 1)
                    {
                        isReadOnly = false;
                        return ImplementationKind.ConfigurationDictionary;
                    }
                }

                if(genericType == typeof(IReadOnlyConfigurationCollection<>))
                {
                    if(type.GetGenericArguments().Length == 1)
                    {
                        isReadOnly = true;
                        return ImplementationKind.ConfigurationCollection;
                    }
                }

                if(genericType == typeof(IConfigurationCollection<>))
                {
                    if(type.GetGenericArguments().Length == 1)
                    {
                        isReadOnly = false;
                        return ImplementationKind.ConfigurationCollection;
                    }
                }
            }

            if(typeof(IConfigurationObject).IsAssignableFrom(type))
            {
                isReadOnly = false;
                return ImplementationKind.ConfigurationObject;
            }

            isReadOnly = false;
            return ImplementationKind.Naive;
        }
    }
}