using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    internal abstract class ReadOnlyConfigurationDictionary<TElement> : ConfigurationDictionaryBase<string, TElement>, IReadOnlyDictionary<string, TElement> where TElement : IConfigurationObject
    {
        public override bool IsReadOnly => true;

        IEnumerable<string> IReadOnlyDictionary<string, TElement>.Keys
        {
            get;
        }
        IEnumerable<TElement> IReadOnlyDictionary<string, TElement>.Values
        {
            get;
        }
    }
}
