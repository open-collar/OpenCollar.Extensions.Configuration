using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public class ConfigurationObjectMock : ConfigurationObjectBase<IRootElement>
    {
        public ConfigurationObjectMock(IConfigurationRoot configurationRoot, IConfigurationParent parent) : base(configurationRoot, parent)
        {
        }

        public ConfigurationObjectMock(PropertyDef? propertyDef, IConfigurationRoot configurationRoot, IConfigurationParent parent) : base(propertyDef, configurationRoot, parent)
        {
        }
    }
}