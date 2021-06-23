using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models.Expense
{
    public class NewExpenseModel
    {
        public SelectedProvider SelectedProvider { get; set; }
        public NewProvider NewProvider { get; set; }
        public List<Items> Items { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SubtotalAmount { get; set; }
    }

    public class NewProvider
    {
        public string Name { get; set; }
        public string Rfc { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

    }

    public class Items
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

    }

    public class SelectedProvider
    {
        public string Rfc { get; set; }
        public Guid Id { get; set; }
    }
}
