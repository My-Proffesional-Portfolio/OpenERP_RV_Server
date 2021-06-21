using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class ReportService : BaseService
    {

        public object GetExpenseBySuppliersAmount(string searchTerm = "", DateTime? emissionStartDate = null, DateTime? emissionEndDate = null)
        {

            var expenses = new ExpenseService().GetFilteredExpenseData(searchTerm, emissionStartDate, emissionEndDate).Include(i=> i.Supplier);
            //https://stackoverflow.com/questions/60872863/the-linq-expression-could-not-be-translated-eiither-rewrite-the-query-in-a-form
            //I assume this is EF Core 3.x.Unfortunately, it is extremely restricted in GroupBy handling -they are planning to improve it in future versions. 
            //    You could put AsEnumerable before the GroupBy, understanding that you will pull all rows across. (PS You can use First instead of FirstOrDefault 
            //    - it isn't possible for there to be an empty group.) – NetMage Mar 26 '20 at 17:50
            var groupedExpenses = expenses.AsEnumerable().GroupBy(g => g.Supplier).Select(s=> new { SupplierName = s.Key.CompanyName, Amount = s.Key.Expenses.Sum(sum=> sum.Total)});

            var response = new List<object>();
            //foreach (var g in groupedExpenses.OrderByDescending(o=> o.Amount).Take(5))
            foreach (var g in groupedExpenses)
            {
                object[] arr = { g.SupplierName, g.Amount };
                response.Add(arr);
            }

            return response;
        
        }
    }
}
