namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// Represents an array of items 
    /// </summary>
    /// <typeparam name="TElement">The type of the array element.  This must be nullable if the type is a reference type and it may be null.</typeparam>
    public interface IPropertyArray<TElement>
    {
        /// <summary>Gets or sets the <see cref="TElement"/> at the specified index.</summary>
        /// <value>The <see cref="TElement"/> at the specified index.</value>
        /// <param name="index">The index of the required element.</param>
        /// <returns></returns>
        TElement this[int index] { get; set; }

        /// <summary>Gets a value indicating whether this array is read-only.</summary>
        /// <value>
        ///   <see langword="true" /> if this array is read-only; otherwise, <see langword="false" />.
        /// </value>
        bool IsReadOnly
        {
            get;
        }
    }
}
