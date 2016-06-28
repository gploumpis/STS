using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STS.Core
{
    public class EmbeddedCertificate : ConfigurationElement
    {
        [ConfigurationProperty("assemblyName")]
        public string AssemblyName
        {
            get
            {
                return base["assemblyName"] as string;
            }
        }

        [ConfigurationProperty("resourceName")]
        public string ResourceName
        {
            get
            {
                return base["resourceName"] as string;
            }
        }

        [ConfigurationProperty("password")]
        public string Password
        {
            get
            {
                return base["password"] as string;
            }
        }
    }
}
