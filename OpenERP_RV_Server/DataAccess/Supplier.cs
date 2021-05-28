using System;
using System.Collections.Generic;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class Supplier
    {
        public Supplier()
        {
            Expenses = new HashSet<Expense>();
        }

        public Guid Id { get; set; }
        public string Rfc { get; set; }
        public string CompanyName { get; set; }
        public string LegalName { get; set; }
        public string AddressLocation { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
        public Guid CompanyId { get; set; }
        public bool? Status { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
    }
}
