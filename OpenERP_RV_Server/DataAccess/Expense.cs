using System;
using System.Collections.Generic;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class Expense
    {
        public Expense()
        {
            ExpenseItems = new HashSet<ExpenseItem>();
        }

        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierRfc { get; set; }
        public string ReceiverRfc { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Tax { get; set; }
        public decimal? AnotherTaxes { get; set; }
        public decimal Total { get; set; }
        public string Xml { get; set; }
        public Guid? Uuid { get; set; }
        public DateTime ExpenseDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string Number { get; set; }
        public string Folio { get; set; }
        public string Cfdiversion { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentTerm { get; set; }
        public string Cfdiuse { get; set; }
        public string AddressLocation { get; set; }

        public virtual Company Company { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<ExpenseItem> ExpenseItems { get; set; }
    }
}
