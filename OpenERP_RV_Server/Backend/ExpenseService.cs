﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public List<ExpenseModel> ProcessFiles(List<IFormFile> files)
        {
            var response = new List<ExpenseModel>();

            foreach (var f in files)
            {
                try
                {
                    response.Add(new ExpenseService().AddExpenseFromCFDI(f, true));
                }
                catch (Exception ex)
                {
                    response.Add(new ExpenseModel { Info = ex.Message });
                    continue;
                }
            }


            return response;
        }

        public ExpenseModel AddExpenseFromCFDI(IFormFile xml, bool saveXML)
        {
            var xmlString = UtilService.ReadFormFileAsync(xml);
            var cfdi = UtilService.Deserialize<Comprobante>(xmlString);
            var companyID = Guid.Parse(accessor.HttpContext.Session.GetString("companyID"));


            if (cfdi.TipoDeComprobante != c_TipoDeComprobante.I)
                throw new Exception("Error en el archivo " + xml.FileName + " : El tipo de comprobante no es válido, debe ser de tipo Ingreso");

            var myRFC = DbContext.Companies.Where(w => w.Id == companyID).FirstOrDefault().FiscalIdentifier;
            if (myRFC != cfdi.Receptor.Rfc)
                throw new Exception("Error en el archivo " + xml.FileName + " : El RFC " + cfdi.Receptor.Rfc + " no coincide con el de usuario");


            var tfd = cfdi.Complemento.SelectMany(sm => sm.Any).Where(w => w.Name.Contains("tfd:TimbreFiscalDigital")).Select(s => s.Attributes);
            var uuid = tfd.ToList().FirstOrDefault().GetNamedItem("UUID").Value;

            if (GetCompanyExpenses().Any(f => f.Uuid == Guid.Parse(uuid)))
                throw new Exception("Error en el archivo " + xml.FileName + " : No se pudo guardar la factura con Folio fiscal " + uuid + " , ya ha sido ingresada anteriormente");



            //var companyID = Guid.Parse(BaseService.companyID);

            var provider = DbContext.Suppliers.Where(w => w.Rfc == cfdi.Emisor.Rfc && w.CompanyId == companyID).FirstOrDefault();

            if (provider == null)
            {
                var newProvider = new Supplier();
                newProvider.Id = Guid.NewGuid();
                newProvider.CompanyId = Guid.Parse(accessor.HttpContext.Session.GetString("companyID"));
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


            var expense = new Expense();
            expense.Id = Guid.NewGuid();
            expense.CompanyId = Guid.Parse(accessor.HttpContext.Session.GetString("companyID"));
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
            {
                var expenseItem = new ExpenseItem()
                {
                    Description = c.Descripcion,
                    Id = Guid.NewGuid(),
                    ExpenseId = expense.Id,
                    Importe = c.Importe,
                    Quantity = c.Cantidad,
                    TotalTaxes = c.Impuestos != null ? c.Impuestos.Traslados.Sum(s => s.Importe) : 0m,
                    Unidad = c.Unidad,
                    UnitPrice = c.ValorUnitario,
                    Discount = c.Descuento,
                };

                DbContext.ExpenseItems.Add(expenseItem);
                DbContext.SaveChanges();
            }

            var response = MapExpenseResponseFromEntity(expense);

            return response;
        }

        public  ExpenseModel GetExpenseById(Guid id)
        {
            var companyID = Guid.Parse(accessor.HttpContext.Session.GetString("companyID"));
            var expense = GetCompanyExpenses().Include(i=> i.Supplier).Include(i2=> i2.ExpenseItems).FirstOrDefault(f => f.CompanyId == companyID && f.Id == id);
            return MapExpenseResponseFromEntity(expense, includeXML: true);

        }


        public PagedListModel<ExpenseModel> GetAllExpenses(int currentPage = 0, int itemsPerPage = 10, string searchTerm = "",
            DateTime? emissionStartDate = null, DateTime? emissionEndDate = null,
             DateTime? creationStartDate = null, DateTime? creationEndDate = null)
        {
            var queryableData = GetCompanyExpenses().OrderBy(o => o.CreationDate);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                queryableData = (IOrderedQueryable<Expense>)queryableData
                    .Where(w => w.Supplier.CompanyName.Contains(searchTerm)
                     || w.SupplierRfc.Contains(searchTerm)
                     || w.Supplier.Email.Contains(searchTerm));

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


        #region helpers

        private IQueryable<Expense> GetCompanyExpenses(Guid? companyID = null)
        {
            if (companyID == null)
            {
                companyID = Guid.Parse(accessor.HttpContext.Session.GetString("companyID"));
            }

            var expenses = DbContext.Expenses.Where(w => w.CompanyId == companyID.Value);
            return expenses;
        }
        private static ExpenseModel MapExpenseResponseFromEntity(Expense expense, bool includeXML = false)
        {
            return new ExpenseModel()
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
                HasXML = !string.IsNullOrWhiteSpace(expense.Xml),
                XML = !string.IsNullOrWhiteSpace(expense.Xml) && includeXML ? expense.Xml: null,
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
        }

        #endregion
    }
}
