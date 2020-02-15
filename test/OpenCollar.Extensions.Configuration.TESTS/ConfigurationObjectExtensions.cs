using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    internal static class ConfigurationObjectExtensions
    {
        internal static string CalculatePath(this IConfigurationObject configurationObject)
        {
            return ((IConfigurationParent)configurationObject).CalculatePath();
        }

        internal static IPropertyDef PropertyDef(this IConfigurationObject configurationObject)
        {
            return ((IConfigurationParent)configurationObject).PropertyDef;
        }
    }
}