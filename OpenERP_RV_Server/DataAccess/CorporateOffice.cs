using System;
using System.Collections.Generic;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class CorporateOffice
    {
        public CorporateOffice()
        {
            Clients = new HashSet<Client>();
            Companies = new HashSet<Company>();
            Products = new HashSet<Product>();
            SalesConcepts = new HashSet<SalesConcept>();
        }

        public Guid Id { get; set; }
        public long CorporativeOfficeNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<SalesConcept> SalesConcepts { get; set; }
    }
}
