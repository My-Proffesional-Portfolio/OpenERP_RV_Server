using Microsoft.AspNetCore.Http;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Models.Expense;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class ExpenseService : BaseService
    {

        public ExpenseModel AddExpenseFromCFDI(IFormFile xml)
        {
            var xmlString = UtilService.ReadFormFileAsync(xml);
            var cfdi = UtilService.Deserialize<Comprobante>(xmlString);

            var provider = DbContext.Suppliers.Where(w => w.Rfc == cfdi.Emisor.Rfc).FirstOrDefault();

            if (provider == null)
            {
                var newProvider = new Supplier();
                newProvider.Id = Guid.NewGuid();
                newProvider.CompanyId = Guid.Parse("F6E973AB-B04C-4414-9597-42D484BBB26D");
                newProvider.AddressLocation = cfdi.LugarExpedicion;
                newProvider.CompanyName = cfdi.Emisor.Nombre;
                newProvider.LegalName = cfdi.Emisor.Nombre;
                newProvider.Email = "NA";
                newProvider.Phone = "NA";
                newProvider.ContactName = "NA";
                newProvider.Rfc = cfdi.Emisor.Rfc;
                DbContext.Suppliers.Add(newProvider);
                DbContext.SaveChanges();

                provider = DbContext.Suppliers.Where(w => w.Rfc == cfdi.Emisor.Rfc).FirstOrDefault();
            }

            var tfd = cfdi.Complemento.SelectMany(sm => sm.Any).Where(w => w.Name.Contains("tfd:TimbreFiscalDigital")).Select(s => s.Attributes);
            var uuid = tfd.ToList().FirstOrDefault().GetNamedItem("UUID").Value;

            var expense = new Expense();
            expense.Id = Guid.NewGuid();
            expense.CompanyId = Guid.Parse("F6E973AB-B04C-4414-9597-42D484BBB26D");
            expense.Xml = xmlString;
            expense.SupplierId = provider.Id;
            expense.Total = cfdi.Total;
            expense.Subtotal = cfdi.SubTotal;
            expense.Uuid = Guid.Parse(uuid);
            expense.Tax = cfdi.Impuestos.TotalImpuestosTrasladados;
            expense.SupplierRfc = cfdi.Emisor.Rfc;
            expense.ReceiverRfc = cfdi.Receptor.Rfc;
            expense.Number = cfdi.Folio;
            expense.Folio = cfdi.Serie;
            expense.ExchangeRate = cfdi.TipoCambio;
            expense.Currency = cfdi.Moneda;
            expense.CreationDate = DateTime.Now;
            expense.ExpenseDate = cfdi.Fecha;

            DbContext.Expenses.Add(expense);
            DbContext.SaveChanges();


            return null;
        }

        public List<ExpenseModel> GetAllExpenses()
        {
            //Stopwatch swQry = new Stopwatch();
            //swQry.Start();
            //var qry = DbContext.Expenses.ToList();
            //swQry.Stop();
            //var timeQry = swQry.ElapsedMilliseconds;

            Stopwatch swQrySelected = new Stopwatch();
            swQrySelected.Start();
            var selectedQry = DbContext.Expenses.Take(500).Select(s => new { s.Xml }).ToList();
            swQrySelected.Stop();
            var timeQrySelected = swQrySelected.ElapsedMilliseconds;

            Stopwatch swQrySelectedNoXML = new Stopwatch();
            swQrySelectedNoXML.Start();
            var qrySelectedNoXML = DbContext.Expenses.Select(s => new { s.Id, s.Total }).ToList();
            swQrySelectedNoXML.Stop();
            var TimeQrySelectedNoXML = swQrySelectedNoXML.ElapsedMilliseconds;

            return null;

        }
    }
}
