using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration.TEMP
{
    public class TempObject : ConfigurationObjectBase, ITempObject
    {
        public TempObject(IConfigurationRoot configurationRoot, IEnumerable<PropertyDef> propertyDefs) : base(configurationRoot, propertyDefs)
        {
        }

        public int ReadOnlyNotNullInt32Property
        {
            get
            {
                return (int)base[nameof(ReadOnlyNotNullInt32Property)];
            }
        }

        public string ReadOnlyNotNullStringProperty
        {
            get
            {
                return (string)base[nameof(ReadOnlyNotNullStringProperty)];
            }
        }

        public int? ReadOnlyNullableInt32Property
        {
            get
            {
                return (int?)base[nameof(ReadOnlyNullableInt32Property)];
            }
        }

        public string? ReadOnlyNullableStringProperty
        {
            get
            {
                return (string?)base[nameof(ReadOnlyNullableStringProperty)];
            }
        }

        public int ReadWriteNotNullInt32Property
        {
            get
            {
                return (int)base[nameof(ReadWriteNotNullInt32Property)];
            }
            set
            {
                base[nameof(ReadWriteNotNullInt32Property)] = value;
            }
        }

        public string ReadWriteNotNullStringProperty
        {
            get
            {
                return (string)base[nameof(ReadWriteNotNullStringProperty)];
            }
            set
            {
                base[nameof(ReadWriteNotNullStringProperty)] = value;
            }
        }

        public int? ReadWriteNullableInt32Property
        {
            get
            {
                return (int?)base[nameof(ReadWriteNullableInt32Property)];
            }
            set
            {
                base[nameof(ReadWriteNullableInt32Property)] = value;
            }
        }

        public string? ReadWriteNullableStringProperty
        {
            get
            {
                return (string?)base[nameof(ReadWriteNullableStringProperty)];
            }
            set
            {
                base[nameof(ReadWriteNullableStringProperty)] = value;
            }
        }
    }
}