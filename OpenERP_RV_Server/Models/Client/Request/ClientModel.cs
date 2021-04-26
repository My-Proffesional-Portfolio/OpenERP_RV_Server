using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models
{
    public class ClientModel 
    {
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string FiscalTaxID { get; set; }
        public Guid CorporateOfficeId { get; set; }
        public Guid? BusinessCategoryID { get; set; }
        public string LegalName { get;  set; }
        public Guid? ClientCompanyStatusId { get;  set; }
        public string DeliveryAddress { get;  set; }
        public Guid Id { get; set; }
        public long Number { get; set; }
        public string FiscalAddress { get;  set; }
    }
   
}
