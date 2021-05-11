using System;
using System.Collections.Generic;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class SalesConcept
    {
        public Guid Id { get; set; }
        public long Number { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public Guid CorporateOfficeId { get; set; }
        public decimal? Cost { get; set; }
        public decimal Price { get; set; }
        public string InternalCode { get; set; }
        public bool? Status { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual CorporateOffice CorporateOffice { get; set; }
    }
}
