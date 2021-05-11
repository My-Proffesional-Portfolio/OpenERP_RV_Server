using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models.SalesConcept
{
    public class ConceptsModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Cost { get; set; }
        public decimal Price { get; set; }
        public long Number { get; set; }
        public string InternalCode { get; set; }
        public Guid CorporateOfficeId { get;  set; }
    }
}
