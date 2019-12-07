using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    internal sealed class ReadOnlyConfigurationCollection<TElement> : ConfigurationDictionaryBase<int, TElement>, IReadOnlyCollection<TElement> where TElement : IConfigurationObject
    {
        public override bool IsReadOnly
        {
            get;
        }

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() => throw new NotImplementedException();
    }
}
