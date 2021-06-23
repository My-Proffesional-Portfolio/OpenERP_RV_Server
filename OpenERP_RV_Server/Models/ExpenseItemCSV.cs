using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models
{
    public class ExpenseItemCSV
    {
        public string Description { get; set; }
        public decimal? Tax { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public Guid ExpenseID { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string ProviderName { get; set; }
        public string ProviderRFC { get; set; }
        public bool HasCFDI { get; set; }

    }
}
