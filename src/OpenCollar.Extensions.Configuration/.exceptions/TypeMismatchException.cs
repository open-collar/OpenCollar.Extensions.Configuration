using System;
using System.Runtime.Serialization;

using Microsoft.Extensions.Configuration;

#pragma warning disable CA1032 // Implement standard exception constructors

namespace OpenCollar.Extensions.Configuration
{
    /// <summary> An exception thrown when an object of the wrong type is added to a collection or assigned to a property. </summary>
    /// <remarks>
    ///     <para> Use the <see cref="ConfigurationException.ConfigurationPath"/> property to discover the path to the configuration item concerned. </para>
    ///     <para>
    ///         The following UML has been generated directly from the source code using
    ///         <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>.
    ///         <img src="../images/uml-diagrams/.exceptions/TypeMismatchException/TypeMismatchException.svg"/>
    ///     </para>
    /// </remarks>
    /// <seealso cref="Exception"/>
    [Serializable]
    public class TypeMismatchException : ConfigurationException
    {
        /// <summary> Initializes a new instance of the <see cref="TypeMismatchException"/> class. </summary>
        /// <param name="configuration"> The configuration service from which data was being read. </param>
        public TypeMismatchException(IConfigurationRoot? configuration) : base(configuration)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="TypeMismatchException"/> class. </summary>
        /// <param name="configuration"> The configuration service from which data was being read. </param>
        /// <param name="message"> The message that describes the error. </param>
        public TypeMismatchException(IConfigurationRoot? configuration, string message) : base(configuration, message)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="TypeMismatchException"/> class. </summary>
        /// <param name="configuration"> The configuration service from which data was being read. </param>
        /// <param name="configurationPath"> The path to the configuration affected. </param>
        /// <param name="message"> The message that describes the error. </param>
        public TypeMismatchException(IConfigurationRoot? configuration, string configurationPath, string message) : base(configuration, configurationPath,
        message)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="TypeMismatchException"/> class. </summary>
        /// <param name="configuration"> The configuration service from which data was being read. </param>
        /// <param name="message"> The error message that explains the reason for the exception. </param>
        /// <param name="innerException"> The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified. </param>
        public TypeMismatchException(IConfigurationRoot? configuration, string message, Exception innerException) : base(configuration, message, innerException)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="TypeMismatchException"/> class. </summary>
        /// <param name="configuration"> The configuration service from which data was being read. </param>
        /// <param name="message"> The error message that explains the reason for the exception. </param>
        /// <param name="configurationPath"> The path to the configuration affected. </param>
        /// <param name="innerException"> The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified. </param>
        public TypeMismatchException(IConfigurationRoot? configuration, string configurationPath, string message, Exception innerException) : base(
        configuration, configurationPath, message, innerException)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="TypeMismatchException"/> class. </summary>
        /// <param name="info"> The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context"> The <see cref="StreamingContext"/> that contains contextual information about the source or destination. </param>
        protected TypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}