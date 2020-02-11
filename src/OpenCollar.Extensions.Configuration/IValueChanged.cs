namespace OpenCollar.Extensions.Configuration
{
    /// <summary>The interface used internally to allow values to call back to their parents to signal that a value has changed.</summary>
    public interface IValueChanged
    {
        /// <summary>Called when a value has changed.</summary>
        /// <param name="oldValue"> The old value. </param>
        /// <param name="newValue"> The new value. </param>
        void OnValueChanged(IValue oldValue, IValue newValue);
    }
}