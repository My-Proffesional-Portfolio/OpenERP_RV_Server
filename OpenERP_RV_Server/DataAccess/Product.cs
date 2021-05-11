using System;
using System.Collections.Generic;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class Product
    {
        public Guid Id { get; set; }
        public long Number { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public Guid CorporateOfficeId { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string BarCode { get; set; }
        public string InternalCode { get; set; }
        public string Weight { get; set; }
        public string Dimentions { get; set; }
        public bool? Status { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual CorporateOffice CorporateOffice { get; set; }
    }
}
