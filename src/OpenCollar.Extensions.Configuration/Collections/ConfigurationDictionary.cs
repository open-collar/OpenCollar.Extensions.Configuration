using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OpenCollar.Extensions.Configuration.Collections;

namespace OpenCollar.Extensions.Configuration
{
    internal sealed class ConfigurationDictionary<TElement> : ConfigurationDictionaryBase<string, TElement>, IConfigurationDictionary<TElement> where TElement : IConfigurationObject
    {
        public override bool IsReadOnly
        {
            get;
        }
    }
}
