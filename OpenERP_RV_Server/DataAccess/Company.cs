using System;
using System.Collections.Generic;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class Company
    {
        public Guid Id { get; set; }
        public Guid CorporateOfficeId { get; set; }
        public string FiscalIdentifier { get; set; }
        public string Phone { get; set; }
        public string CommercialName { get; set; }
        public string LegalName { get; set; }
        public string Address { get; set; }
        public Guid BusinessCategoryId { get; set; }
        public bool Status { get; set; }
        public string OfficeNumberId { get; set; }

        public virtual BusinessCategory BusinessCategory { get; set; }
        public virtual CorporateOffice CorporateOffice { get; set; }
    }
}
