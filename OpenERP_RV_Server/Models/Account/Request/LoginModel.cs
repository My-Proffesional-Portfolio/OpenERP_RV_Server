using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models.Account.Request
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool SpecialRequestToken { get; set; }
    }
}
