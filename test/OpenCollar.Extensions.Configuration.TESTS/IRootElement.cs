using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public interface IRootElement : IConfigurationObject
    {
        // We would expect this to be implemented as a read-only property.
        int Int32PropertyA
        {
            get;
        }

        // We would expect this to be implemented as a read/write property.
        int Int32PropertyB
        {
            get; set;
        }

        // This should be a 
        IConfigurationCollection<IChildElement> ChildElements
        {
            get;
        }
    }
}
