using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration.TEMP
{
    public interface ITempObject
    {
        int ReadOnlyNotNullInt32Property
        {
            get;
        }

        string ReadOnlyNotNullStringProperty
        {
            get;
        }

        int? ReadOnlyNullableInt32Property
        {
            get;
        }

        string? ReadOnlyNullableStringProperty
        {
            get;
        }

        int ReadWriteNotNullInt32Property
        {
            get; set;
        }

        string ReadWriteNotNullStringProperty
        {
            get; set;
        }

        int? ReadWriteNullableInt32Property
        {
            get; set;
        }

        string? ReadWriteNullableStringProperty
        {
            get; set;
        }
    }
}