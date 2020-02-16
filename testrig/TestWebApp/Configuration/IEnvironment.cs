using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenCollar.Extensions.Configuration;

namespace TestWebApp.Configuration
{
    public interface IEnvironment : IConfigurationObject
    {
        public string EnvironmentName
        {
            get;
        }

        public string Version
        {
            get;
        }
    }
}