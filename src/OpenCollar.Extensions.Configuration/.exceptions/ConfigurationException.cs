using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Microsoft.Extensions.Configuration;

using OpenCollar.Extensions.Validation;

#pragma warning disable CA1032 // Implement standard exception constructors

namespace OpenCollar.Extensions.Configuration
{
    /// <summary> An exception thrown when an error occurs during the reading or writing of configuration. </summary>
    /// <remarks>
    ///     <para> Use the <see cref="ConfigurationPath"/> property to discover the path to the configuration item concerned. </para>
    ///     <para>
    ///         The following UML has been generated directly from the source code using
    ///         <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>.
    ///         <img src="../images/uml-diagrams/.exceptions/ConfigurationException/ConfigurationException.svg"/>
    ///     </para>
    /// </remarks>
    /// <seealso cref="Exception"/>
    [Serializable]
    public class ConfigurationException : Exception
    {
        /// <summary> Initializes a new instance of the <see cref="ConfigurationException"/> class. </summary>
        /// <param name="configuration"> The configuration service from which data was being read. </param>
        public ConfigurationException(IConfigurationRoot? configuration) : base(GetMessage(configuration, null, out var loadedConfiguration))
        {
            LoadedConfiguration = loadedConfiguration;
        }

        /// <summary> Initializes a new instance of the <see cref="ConfigurationException"/> class. </summary>
        /// <param name="configuration"> The configuration service from which data was being read. </param>
        /// <param name="message"> The message that describes the error. </param>
        public ConfigurationException(IConfigurationRoot? configuration, string message) : base(GetMessage(configuration, message, out var loadedConfiguration))
        {
            LoadedConfiguration = loadedConfiguration;
        }

        /// <summary> Initializes a new instance of the <see cref="ConfigurationException"/> class. </summary>
        /// <param name="configuration"> The configuration service from which data was being read. </param>
        /// <param name="configurationPath"> The path to the configuration affected. </param>
        /// <param name="message"> The message that describes the error. </param>
        public ConfigurationException(IConfigurationRoot? configuration, string? configurationPath, string message) : base(GetMessage(configuration, message,
        out var loadedConfiguration))
        {
            LoadedConfiguration = loadedConfiguration;
            ConfigurationPath = configurationPath;
        }

        /// <summary> Initializes a new instance of the <see cref="ConfigurationException"/> class. </summary>
        /// <param name="configuration"> The configuration service from which data was being read. </param>
        /// <param name="message"> The error message that explains the reason for the exception. </param>
        /// <param name="innerException"> The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified. </param>
        public ConfigurationException(IConfigurationRoot? configuration, string message, Exception innerException) : base(
        GetMessage(configuration, message, out var loadedConfiguration), innerException)
        {
            LoadedConfiguration = loadedConfiguration;
        }

        /// <summary> Initializes a new instance of the <see cref="ConfigurationException"/> class. </summary>
        /// <param name="configuration"> The configuration service from which data was being read. </param>
        /// <param name="message"> The error message that explains the reason for the exception. </param>
        /// <param name="configurationPath"> The path to the configuration affected. </param>
        /// <param name="innerException"> The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified. </param>
        public ConfigurationException(IConfigurationRoot? configuration, string? configurationPath, string message, Exception innerException) : base(
        GetMessage(configuration, message, out var loadedConfiguration), innerException)
        {
            LoadedConfiguration = loadedConfiguration;
            ConfigurationPath = configurationPath;
        }

        /// <summary> Initializes a new instance of the <see cref="ConfigurationException"/> class. </summary>
        /// <param name="info"> The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context"> The <see cref="StreamingContext"/> that contains contextual information about the source or destination. </param>
        protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ConfigurationPath = info.GetString(nameof(ConfigurationPath));
            LoadedConfiguration = info.GetString(nameof(LoadedConfiguration));
        }

        /// <summary> Gets or sets the path to the configuration affected. </summary>
        /// <value> The path to the configuration affected. </value>
        public string? ConfigurationPath { get; set; }

        /// <summary> Gets or sets the details of the loaded configuration providers at the time of the error. </summary>
        /// <value> The details of the loaded configuration providers at the time of the error. </value>
        public string? LoadedConfiguration { get; set; }

        /// <summary> When overridden in a derived class, sets the <see cref="SerializationInfo"/> with information about the exception. </summary>
        /// <param name="info"> The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context"> The <see cref="StreamingContext"/> that contains contextual information about the source or destination. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="info"/> is <see langword="null"/>. </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.Validate(nameof(info), ObjectIs.NotNull);

            info.AddValue(nameof(ConfigurationPath), ConfigurationPath);
            info.AddValue(nameof(LoadedConfiguration), LoadedConfiguration);
            base.GetObjectData(info, context);
        }

        /// <summary> Appends the keys in a provider below the root key specified. </summary>
        /// <param name="provider"> The provider. </param>
        /// <param name="rootKey"> The root key. </param>
        /// <param name="builder"> The builder to which to append the keys. </param>
        /// <param name="path"> The path to the current key (can be empty if root). </param>
        private static void AppendKeys(IConfigurationProvider provider, string? rootKey, StringBuilder builder, string path)
        {
            List<string> keys = string.IsNullOrWhiteSpace(path)
            ? provider.GetChildKeys(Enumerable.Empty<string>(), rootKey).ToList()
            : provider.GetChildKeys(Enumerable.Empty<string>(), path).ToList();

            if(keys.Count <= 0)
            {
                builder.Append("    ");

                builder.AppendLine(!string.IsNullOrWhiteSpace(path) ? path : rootKey);

                return;
            }

            foreach(var key in keys.Distinct())
            {
                AppendKeys(provider, key, builder, !string.IsNullOrWhiteSpace(path) ? string.Concat(path, ":", key) : key);
            }
        }

        /// <summary>
        /// Initializes the the details of the loaded configuration providers at the time of the error and returns the message with that information
        /// appended.
        /// </summary>
        /// <param name="configuration">The configuration service from which data was being read.</param>
        /// <param name="message">The error message to which to append details of the loaded configuration.</param>
        /// <param name="loadedConfiguration">An argument in which the details of the loaded configuration are returned.</param>
        /// <returns></returns>
        private static string? GetMessage(IConfigurationRoot? configuration, string? message, out string? loadedConfiguration)
        {
            if(configuration is null)
            {
                loadedConfiguration = "[NULL]";
                return message;
            }

            var builder = new StringBuilder();
            var n = 0;
            foreach(var provider in configuration.Providers)
            {
                builder.Append("  [");
                builder.Append(n++);
                builder.Append("]: \"");
                builder.Append(provider);
                builder.AppendLine("\"");
                AppendKeys(provider, null, builder, string.Empty);
            }

            loadedConfiguration = builder.ToString();

            if(string.IsNullOrWhiteSpace(message))
            {
                return "Unhandled configuration exception.\r\nConfigurationRoot:\r\n" + loadedConfiguration;
            }

            return message + "\r\nConfigurationRoot:\r\n" + loadedConfiguration;
        }
    }
}