using System;
using System.Runtime.Serialization;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     An exception thrown when an object of the wrong type is added to a collection or assigned to a property.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/exceptions/TypeMismatchException/TypeMismatchException.svg" />
    /// </remarks>
    /// <seealso cref="Exception" />
    [Serializable]
    public class TypeMismatchException : ConfigurationException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeMismatchException" /> class.
        /// </summary>
        public TypeMismatchException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeMismatchException" /> class.
        /// </summary>
        /// <param name="message">
        ///     The message that describes the error.
        /// </param>
        public TypeMismatchException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeMismatchException" /> class.
        /// </summary>
        /// <param name="configurationPath">
        ///     The path to the configuration affected.
        /// </param>
        /// <param name="message">
        ///     The message that describes the error.
        /// </param>
        public TypeMismatchException(string configurationPath, string message) : base(configurationPath, message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeMismatchException" /> class.
        /// </summary>
        /// <param name="message">
        ///     The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or <see langword="null" /> if no inner
        ///     exception is specified.
        /// </param>
        public TypeMismatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeMismatchException" /> class.
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
        public TypeMismatchException(string configurationPath, string message, Exception innerException) : base(configurationPath, message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeMismatchException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="StreamingContext" /> that contains contextual information about the source or destination.
        /// </param>
        protected TypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}