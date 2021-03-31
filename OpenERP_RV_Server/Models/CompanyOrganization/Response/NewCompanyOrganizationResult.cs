using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models.CompanyOrganization
{
    public class NewCompanyOrganizationResult : BaseResponse
    {
        public string UserName { get; set; }
        public string UserToken { get; set; }
        public long CorporateOfficeNumber { get; set; }
        public string LegalName { get; set; }

        public DateTime UserTokenExpiration { get; set; }
        public bool SucceedCreation
        {
            get
            {
                return this.ErrorMessages.Count == 0;
            }
        }

    }
}
