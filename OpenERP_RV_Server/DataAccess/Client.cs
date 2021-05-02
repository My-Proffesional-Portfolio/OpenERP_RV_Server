using System;
using System.Collections.Generic;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class Client
    {
        public Guid Id { get; set; }
        public long Number { get; set; }
        public string CompanyName { get; set; }
        public string LegalName { get; set; }
        public string ContactName { get; set; }
        public string FiscalIdentifier { get; set; }
        public string FiscalAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public Guid CorporateOfficeId { get; set; }
        public Guid BusinessCategoryId { get; set; }
        public Guid? ClientCompanyStatusId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public virtual BusinessCategory BusinessCategory { get; set; }
        public virtual CorporateOffice CorporateOffice { get; set; }
    }
}
