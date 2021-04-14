using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server
{
    public class AppSettings
    {
        public Security Jwt { get; set; }
        public string AllowedHosts { get; set; }
    }

    public class Security
    {
        public string JWT_PrivateKey { get; set; }
        public string issuer { get; set; }
        public string audicence { get; set; }
    }
}
