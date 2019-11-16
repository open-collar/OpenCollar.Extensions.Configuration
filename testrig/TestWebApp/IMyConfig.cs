using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp
{
    public interface IMyConfig
    {
        public string ReadOnlyString { get; }

        public string ReadWriteString { get; set; }
    }
}
