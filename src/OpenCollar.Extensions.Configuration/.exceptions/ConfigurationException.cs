using System;
using System.Runtime.Serialization;

using OpenCollar.Extensions.Configuration.Validation;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     An exception thrown when an error occurs during the reading or writing of configuration.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/.exceptions/ConfigurationException/ConfigurationException.svg" />
    /// </remarks>
    /// <seealso cref="Exception" />
    [Serializable]
    public class ConfigurationException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationException" /> class.
        /// </summary>
        public ConfigurationException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationException" /> class.
        /// </summary>
        /// <param name="message">
        ///     The message that describes the error.
        /// </param>
        public ConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationException" /> class.
        /// </summary>
        /// <param name="configurationPath">
        ///     The path to the configuration affected.
        /// </param>
        /// <param name="message">
        ///     The message that describes the error.
        /// </param>
        public ConfigurationException(string? configurationPath, string message) : base(message)
        {
            ConfigurationPath = configurationPath;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationException" /> class.
        /// </summary>
        /// <param name="message">
        ///     The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or <see langword="null" /> if no inner
        ///     exception is specified.
        /// </param>
        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationException" /> class.
        /// </summary>
        /// <param name="message">
        ///     The error message that explains the reason for the exception.
        /// </param>
        /// <param name="configurationPath">
        ///     The path to the configuration affected.
        /// </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or <see langword="null" /> if no inner
        ///     exception is specified.
        /// </param>
        public ConfigurationException(string? configurationPath, string message, Exception innerException) : base(message, innerException)
        {
            ConfigurationPath = configurationPath;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="StreamingContext" /> that contains contextual information about the source or destination.
        /// </param>
        protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ConfigurationPath = info.GetString(nameof(ConfigurationPath));
        }

        /// <summary>
        ///     Gets or sets the path to the configuration affected.
        /// </summary>
        /// <value>
        ///     The path to the configuration affected.
        /// </value>
        public string? ConfigurationPath
        {
            get; set;
        }

        /// <summary>
        ///     When overridden in a derived class, sets the <see cref="SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="StreamingContext" /> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="info" /> is <see langword="null" />.
        /// </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.Validate(nameof(info), ObjectIs.NotNull);

            info.AddValue(nameof(ConfigurationPath), ConfigurationPath);
            base.GetObjectData(info, context);
        }
    }
}