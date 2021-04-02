using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models.Account.Response
{
    public class LoginSessionData
    {
        public string CurrentToken { get; set; }
        public string CurrentUserName { get; set; }
        public string CompanyLogged { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}
