using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace OpenERP_RV_Server.Models.Account.Request
{
    public class LoginResponseModel : BaseResponse
    {

        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string CompanyID { get; set; }
        public string CorporateOfficeID { get; set; }
        public string UserName { get; set; }
        public string CompanyLegalName { get; set; }
    }
}
