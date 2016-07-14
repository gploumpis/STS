using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STS.Simple.Core
{
    public class STSConfiguration<T> : ConfigurationSection where T : ConfigurationElement, IRelyingParty, new()
    {
        [ConfigurationProperty("relyingParties")]
        public RelyingParties<T> RelyingParties
        {
            get { return base["relyingParties"] as RelyingParties<T>; }
        }

        public static STSConfiguration<T> Current
        {
            get
            {
                return ConfigurationManager.GetSection("sts.configuration") as STSConfiguration<T>;
            }
        }

    }
}
