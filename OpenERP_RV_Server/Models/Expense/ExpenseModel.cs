using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models.Expense
{
    public class ExpenseModel : BaseResponse
    {
        public Guid Id { get; set; }
        public Guid CompanyID { get; set; }
        public decimal Total { get; set; }
        public decimal Subtotal { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
