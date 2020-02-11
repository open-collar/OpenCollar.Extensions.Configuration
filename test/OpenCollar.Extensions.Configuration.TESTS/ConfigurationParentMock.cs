using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class ConfigurationParentMock : IConfigurationParent
    {
        public bool IsReadOnly
        {
            get; set;
        }

        public string Path
        {
            get; set;
        }

        public string GetPath()
        {
            return Path;
        }
    }
}