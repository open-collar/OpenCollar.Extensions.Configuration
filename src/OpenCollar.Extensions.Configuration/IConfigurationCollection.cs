namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// Represents a collection of values stored in a property.
    /// </summary>
    /// <typeparam name="TElement">The type of the array element.  This must be nullable if the type is a reference type and can be <see langword="null"/>.</typeparam>
    public interface IConfigurationCollection<TElement>:System.Collections.Generic.IList<TElement>, System.Collections.Specialized.INotifyCollectionChanged, INotifyItemChanged where TElement : IConfigurationObject
    {
    }
}
