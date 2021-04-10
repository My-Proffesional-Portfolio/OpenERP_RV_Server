using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models.CompanyOrganization
{
    public class NewCompanyOrganizationModel
    {
        public string LegalName { get; set; }
        public string CommercialName { get; set; }
        public string FiscalIdentificationNumber { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string OfficeNumberId { get; set; }
        public string ContactName { get; set; }

    }
}
