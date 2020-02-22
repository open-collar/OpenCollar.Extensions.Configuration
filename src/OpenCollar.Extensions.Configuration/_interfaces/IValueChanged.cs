using System.ComponentModel;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     The interface used internally to allow values to call back to their parents to signal that a value has changed.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/interfaces/IValueChanged/IValueChanged.svg" />
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IValueChanged
    {
        /// <summary>
        ///     Called when a value has changed.
        /// </summary>
        /// <param name="oldValue">
        ///     The old value.
        /// </param>
        /// <param name="newValue">
        ///     The new value.
        /// </param>
        void OnValueChanged(IValue oldValue, IValue newValue);
    }
}