using System;
using System.Collections.Generic;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class CorporateOffice
    {
        public CorporateOffice()
        {
            Companies = new HashSet<Company>();
        }

        public Guid Id { get; set; }
        public long CorporativeOfficeNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Company> Companies { get; set; }
    }
}
