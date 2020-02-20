using System;
using System.Runtime.Serialization;

using OpenCollar.Extensions.Configuration.Validation;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     An exception thrown when a property is incorrectly defined..
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/_exceptions/InvalidPropertyException/InvalidPropertyException.svg" />
    /// </remarks>
    /// <seealso cref="Exception" />
    [Serializable]
    public class InvalidPropertyException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidPropertyException" /> class.
        /// </summary>
        public InvalidPropertyException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidPropertyException" /> class.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        public InvalidPropertyException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidPropertyException" /> class.
        /// </summary>
        /// <param name="propertyName"> The property name and parent type name of the property affected. </param>
        /// <param name="message"> The message that describes the error. </param>
        public InvalidPropertyException(string? propertyName, string message) : base(message)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidPropertyException" /> class.
        /// </summary>
        /// <param name="message"> The error message that explains the reason for the exception. </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or <see langword="null" /> if no inner
        ///     exception is specified.
        /// </param>
        public InvalidPropertyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidPropertyException" /> class.
        /// </summary>
        /// <param name="message"> The error message that explains the reason for the exception. </param>
        /// <param name="propertyName"> The property name and parent type name of the property affected. </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or <see langword="null" /> if no inner
        ///     exception is specified.
        /// </param>
        public InvalidPropertyException(string? propertyName, string message, Exception innerException) : base(message, innerException)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidPropertyException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="StreamingContext" /> that contains contextual information about the source or destination.
        /// </param>
        protected InvalidPropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            PropertyName = info.GetString(nameof(PropertyName));
        }

        /// <summary>
        ///     Gets or sets the property name and parent type name of the property affected.
        /// </summary>
        /// <value> The property name and parent type name of the property affected. </value>
        public string? PropertyName
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
        /// <exception cref="ArgumentNullException"> <paramref name="info" /> is <see langword="null" />. </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.Validate(nameof(info), ObjectIs.NotNull);

            info.AddValue(nameof(PropertyName), PropertyName);
            base.GetObjectData(info, context);
        }
    }
}