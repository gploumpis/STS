using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STS.Simple.Core
{
    
    public class STSConfiguration : STSConfiguration<RelyingParty>
    {
        public static STSConfiguration Current
        {
            get
            {
                return ConfigurationManager.GetSection("sts.configuration") as STSConfiguration;
            }
        }

    }
}
