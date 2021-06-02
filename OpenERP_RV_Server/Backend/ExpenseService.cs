﻿using Microsoft.AspNetCore.Http;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Models.Expense;
using OpenERP_RV_Server.Models.PagedModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class ExpenseService : BaseService
    {

        public ExpenseModel AddExpenseFromCFDI(IFormFile xml, bool saveXML)
        {
            var xmlString = UtilService.ReadFormFileAsync(xml);
            var cfdi = UtilService.Deserialize<Comprobante>(xmlString);

            var companyID = Guid.Parse(HttpContext.Session.GetString("companyID"));
            //var companyID = Guid.Parse(BaseService.companyID);

            var provider = DbContext.Suppliers.Where(w => w.Rfc == cfdi.Emisor.Rfc && w.CompanyId == companyID).FirstOrDefault();

            if (provider == null)
            {
                var newProvider = new Supplier();
                newProvider.Id = Guid.NewGuid();
                newProvider.CompanyId = Guid.Parse(HttpContext.Session.GetString("companyID"));
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
            expense.CompanyId = Guid.Parse(HttpContext.Session.GetString("companyID"));
            expense.Xml = saveXML ? xmlString : null;
            expense.SupplierId = provider.Id;
            expense.Total = cfdi.Total;
            expense.Subtotal = cfdi.SubTotal;
            expense.Uuid = Guid.Parse(uuid);
            expense.Tax = cfdi.Impuestos?.TotalImpuestosTrasladados;
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

            foreach (var c in cfdi.Conceptos)
            //for (int i = 0; i < 10; i++)
            {
                //var c = cfdi.Conceptos[0];
                var expenseItem = new ExpenseItem()
                {
                    Description = c.Descripcion,
                    Id = Guid.NewGuid(),
                    ExpenseId = expense.Id,
                    Importe = c.Importe,
                    Quantity = c.Cantidad,
                    TotalTaxes = c.Impuestos.Traslados.Sum(s => s.Importe),
                    Unidad = c.Unidad,
                    UnitPrice = c.ValorUnitario,
                    Discount = c.Descuento,
                };

                DbContext.ExpenseItems.Add(expenseItem);
                DbContext.SaveChanges();
            }


            var response = new ExpenseModel()
            {
                CurrencyCode = expense.Currency,
                Total = expense.Total,
                Subtotal = expense.Subtotal.HasValue ? expense.Subtotal.Value : 0m,
                Taxes = expense.Tax.HasValue ? expense.Tax.Value : 0m,
                SupplierName = expense.Supplier.CompanyName,
                SupplierTaxID = expense.SupplierRfc,
                ReceiverTaxID = expense.ReceiverRfc,
                ExchangeRate = expense.ExchangeRate,
                CompanyID = expense.CompanyId,
                ExpenseDate = expense.ExpenseDate,
                Id = expense.Id,
                ExpenseItems = expense.ExpenseItems.Select(
                    s => new ExpenseItemModel
                    {
                        Id = s.Id,
                        Total = s.Importe,
                        Description = s.Description,
                        Quantity = s.Quantity,
                        Subtotal = s.UnitPrice * s.Quantity,
                        Taxes = s.TotalTaxes.HasValue ? s.TotalTaxes.Value : 0m,
                        UnitPrice = s.UnitPrice,
                        ExpenseID = s.ExpenseId
                    }).ToList(),

            };

            return response;
        }

        //internal object AddExpenseFromCFDIBatch(IFormFile files)
        //{
        //    using (var stream = files.OpenReadStream())
        //    using (var archive = new ZipArchive(stream))
        //    {
        //        var innerFile = archive.GetEntry("foo.txt");
        //        // do something with the inner file
        //    }
        //}

        public PagedListModel<ExpenseModel> GetAllExpenses(int currentPage = 0, int itemsPerPage = 10)
        {
            var queryableData = GetCompanyExpenses().OrderBy(o => o.CreationDate);
            var pagedExpenses = queryableData.GetPagedData(currentPage, itemsPerPage);

            var expenses = pagedExpenses.Select(s => new ExpenseModel
            {
                Id = s.Id,
                SupplierName = s.Supplier.CompanyName,
                Subtotal = s.Subtotal.HasValue ? s.Subtotal.Value : 0m,
                CurrencyCode = s.Currency,
                Total = s.Total,
                CompanyID = s.CompanyId,
                ExpenseDate = s.ExpenseDate,
                SupplierTaxID = s.SupplierRfc,
                ReceiverTaxID = s.ReceiverRfc,
                CreationDate = s.CreationDate,
                Taxes = s.Tax.HasValue ? s.Tax.Value : 0m,
                HasXML = !string.IsNullOrWhiteSpace(s.Xml)

            }).ToList();

            var response = UtilService.GetPagedEntityModel(itemsPerPage, queryableData, expenses);
            response.Total = queryableData.Sum(s => s.Total);
            response.Subtotal = queryableData.Sum(s => s.Subtotal ?? 0m);
            return response;

        }

        private IQueryable<Expense> GetCompanyExpenses(Guid? companyID = null)
        {
            if (companyID == null)
            {
                companyID = Guid.Parse(HttpContext.Session.GetString("companyID"));
            }

            var expenses = DbContext.Expenses.Where(w => w.CompanyId == companyID.Value);
            return expenses;
        }
    }
}
