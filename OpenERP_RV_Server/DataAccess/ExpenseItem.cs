using System;
using System.Collections.Generic;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class ExpenseItem
    {
        public Guid Id { get; set; }
        public Guid ExpenseId { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Importe { get; set; }
        public decimal Quantity { get; set; }
        public string Unidad { get; set; }
        public decimal? TotalTaxes { get; set; }

        public virtual Expense Expense { get; set; }
    }
}
