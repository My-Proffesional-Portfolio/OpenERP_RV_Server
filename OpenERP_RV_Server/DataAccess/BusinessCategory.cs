using System;
using System.Collections.Generic;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class BusinessCategory
    {
        public BusinessCategory()
        {
            Clients = new HashSet<Client>();
            Companies = new HashSet<Company>();
        }

        public Guid Id { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
    }
}
