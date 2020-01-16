using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration.TESTS.Implementations
{
    internal sealed class ConfigurationObjectA : ConfigurationObjectBase<IRootElement>, IRootElement
    {
        public ConfigurationObjectA(IConfigurationRoot configurationRoot) : base(configurationRoot)
        {
        }

        public ConfigurationObjectA(IConfigurationRoot configurationRoot, IEnumerable<PropertyDef> propertyDefs) : base(configurationRoot)
        {
        }

        public IConfigurationCollection<IChildElement> ChildElements
        {
            get
            {
                return (IConfigurationCollection<IChildElement>)base[nameof(ChildElements)];
            }
        }

        public int Int32PropertyA
        {
            get
            {
                return (int)base[nameof(Int32PropertyA)];
            }
        }

        public int Int32PropertyB
        {
            get
            {
                return (int)base[nameof(Int32PropertyB)];
            }
            set
            {
                base[nameof(Int32PropertyB)] = value;
            }
        }
    }
}