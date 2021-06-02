using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models.Expense
{
    public class ExpenseModel : BaseResponse
    {
        public ExpenseModel()
        {
            ExpenseItems = new List<ExpenseItemModel>();
        }
        public Guid Id { get; set; }
        public Guid CompanyID { get; set; }
        public decimal Total { get; set; }
        public decimal Subtotal { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
        public string SupplierName { get; set; }
        public string SupplierTaxID { get; set; }
        public string ReceiverTaxID { get; set; }
        public List<ExpenseItemModel> ExpenseItems { get; set; }
        public decimal Taxes { get; internal set; }
        public DateTime ExpenseDate { get;  set; }
        public DateTime CreationDate { get; internal set; }
        public bool HasXML { get; internal set; }
    }

    public class ExpenseItemModel {

        public Guid Id { get; set; }
        public Guid ExpenseID { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Taxes { get; set; }
        public decimal Total { get; set; }
    }
}
