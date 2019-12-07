using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// Defines a dictionary containing configurtation items and keyed on the element name.
    /// </summary>
    /// <typeparam name="TElement">The type of the elements contained in the dictionary.</typeparam>
    /// <seealso cref="System.Collections.Generic.IDictionary{Type,T}" />
    public interface IConfigurationDictionary<TElement>:IDictionary<string, TElement>, System.Collections.Specialized.INotifyCollectionChanged, INotifyItemChanged where TElement : IConfigurationObject
    {
    }
}
