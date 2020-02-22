using System.ComponentModel;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Defines the interface common to a objects that may belong to a configuration object.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/interfaces/IConfigurationChild/IConfigurationChild.svg" />
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal interface IConfigurationChild
    {
        /// <summary>
        ///     Sets the parent of a configuration object.
        /// </summary>
        /// <param name="parent">
        ///     The new parent object.
        /// </param>
        void SetParent(IConfigurationParent? parent);
    }
}