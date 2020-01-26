namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     The interface used internally to allow values to call back to their parents to signal that a value has changed.
    /// </summary>
    internal interface IValueChanged
    {
        /// <summary>
        ///     Called when a value has changed.
        /// </summary>
        /// <param name="value"> The value that has changed. </param>
        void OnValueChanged(ValueBase value);
    }
}