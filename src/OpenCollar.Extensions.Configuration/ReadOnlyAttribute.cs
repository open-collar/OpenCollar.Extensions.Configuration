using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// An attribute that can be used to indicate that a property (especially arrays and dictionaries) should be
    /// treated as read-only.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ReadOnlyAttribute:Attribute
    {
    }
}
