using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenCollar.Extensions.Configuration;

namespace TestWebApp
{
    public interface IMyConfig:IConfigurationObject
    {
        public string ReadOnlyString { get; }

        public string ReadWriteString { get; set; }
    }
}
