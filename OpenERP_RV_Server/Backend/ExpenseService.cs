using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Models;
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

            if (cfdi.Version != "3.3")
                throw new Exception("Error en el archivo " + xml.FileName + " : El archivo cargado es un archivo CFDI 3.2 el cuál ya no es válido");

            if (cfdi.TipoDeComprobante != c_TipoDeComprobante.I)
                throw new Exception("Error en el archivo " + xml.FileName + " : El tipo de comprobante no es válido, debe ser de tipo Ingreso");

            var myRFC = DbContext.Companies.Where(w => w.Id == companyID).FirstOrDefault().FiscalIdentifier;
            if (myRFC != cfdi.Receptor.Rfc)
                throw new Exception("Error en el archivo " + xml.FileName + " : El RFC " + cfdi.Receptor.Rfc + " no coincide con el de usuario");



            var tfd = cfdi.Complemento.SelectMany(sm => sm.Any).Where(w => w.Name.Contains("tfd:TimbreFiscalDigital")).Select(s => s.Attributes);
            var uuid = tfd.ToList().FirstOrDefault().GetNamedItem("UUID").Value;

            if (GetCompanyExpenses().Any(f => f.Uuid == Guid.Parse(uuid)))
                throw new Exception("Error en el archivo " + xml.FileName + " : No se pudo guardar la factura con Folio fiscal " + uuid + " , ya ha sido ingresada anteriormente");


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

                provider = DbContext.Suppliers.Where(w => w.Rfc == cfdi.Emisor.Rfc && w.CompanyId == companyID).FirstOrDefault();
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
            expense.PaymentMethod = cfdi.MetodoPago == (c_MetodoPago)0 ? "PUE" : "PPD";

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
                    FullFilled = true
                };

                DbContext.ExpenseItems.Add(expenseItem);
                DbContext.SaveChanges();
            }

            var response = MapExpenseResponseFromEntity(expense);

            return response;
        }

        public bool UpdateExpenseItemStatus(bool statusUpdate, Guid uuid)
        {
            var succeed = false;
            var item = DbContext.ExpenseItems.Where(w => w.Expense.CompanyId == Guid.Parse(accessor.HttpContext.Session.GetString("companyID"))
           && w.Id == uuid).FirstOrDefault();

            if (item == null)
                return succeed;

            item.FullFilled = statusUpdate;
            DbContext.SaveChanges();

            return succeed = true;
        }

        public List<ExpenseModel> AddExpense(NewExpenseModel newExpense)
        {
            var newProvider = new Supplier();
            var provider = new Supplier();

            if (newExpense.SelectedProvider == null)
            {
                provider = DbContext.Suppliers.Where(w => w.Rfc == newExpense.NewProvider.Rfc && w.CompanyId == Guid.Parse(accessor.HttpContext.Session.GetString("companyID"))).FirstOrDefault();
                if (provider == null)
                {

                    newProvider.Id = Guid.NewGuid();
                    newProvider.CompanyId = Guid.Parse(accessor.HttpContext.Session.GetString("companyID"));
                    newProvider.AddressLocation = newExpense.NewProvider.Address;
                    newProvider.CompanyName = newExpense.NewProvider.Name;
                    newProvider.LegalName = newExpense.NewProvider.Name;
                    newProvider.Email = newExpense.NewProvider.Email;
                    newProvider.Phone = "NA";
                    newProvider.ContactName = "NA";
                    newProvider.Rfc = newExpense.NewProvider.Rfc;
                    DbContext.Suppliers.Add(newProvider);
                    DbContext.SaveChanges();
                    provider = DbContext.Suppliers.Where(w => w.Rfc == newProvider.Rfc && w.CompanyId == newProvider.CompanyId).FirstOrDefault();
                }

            }

            var response = new List<ExpenseModel>();
            var newExpenseInDB = new Expense();
            newExpenseInDB.Id = Guid.NewGuid();
            newExpenseInDB.CompanyId = Guid.Parse(accessor.HttpContext.Session.GetString("companyID"));
            newExpenseInDB.Total = newExpense.TotalAmount;
            newExpenseInDB.Tax = newExpense.TaxAmount;
            newExpenseInDB.Subtotal = newExpense.SubtotalAmount;
            newExpenseInDB.Cfdiversion = "";
            newExpenseInDB.Cfdiuse = "";
            newExpenseInDB.Xml = "";
            newExpenseInDB.SupplierId = newExpense.SelectedProvider != null ? newExpense.SelectedProvider.Id : provider.Id;
            newExpenseInDB.SupplierRfc = newExpense.SelectedProvider != null ? newExpense.SelectedProvider.Rfc : provider.Rfc;
            newExpenseInDB.Uuid = null;
            newExpenseInDB.Number = "NA/" + newExpense.ExpenseDate.Year + "-" + newExpense.ExpenseDate.Month + "-" + newExpense.ExpenseDate.Day;
            newExpenseInDB.Folio = "MANUAL/" + (newExpense.SelectedProvider != null ? newExpense.SelectedProvider.Rfc : provider.Rfc);
            newExpenseInDB.CreationDate = DateTime.Now;
            newExpenseInDB.ExchangeRate = 1;
            newExpenseInDB.Currency = "MXN";
            newExpenseInDB.ExpenseDate = newExpense.ExpenseDate;
            DbContext.Expenses.Add(newExpenseInDB);

            foreach (var item in newExpense.Items)
            {
                var newExpenseItem = new ExpenseItem();
                newExpenseItem.Id = Guid.NewGuid();
                newExpenseItem.ExpenseId = newExpenseInDB.Id;
                newExpenseItem.Unidad = "PIEZA";
                newExpenseItem.Importe = item.Quantity * item.Price;
                newExpenseItem.FullFilled = item.FullFilled;
                newExpenseItem.TotalTaxes = item.Tax;
                newExpenseItem.Description = item.ProductName;
                newExpenseItem.Discount = 0m;
                newExpenseItem.UnitPrice = item.Price;
                newExpenseItem.Quantity = item.Quantity;
                DbContext.ExpenseItems.Add(newExpenseItem);
            }

            DbContext.SaveChanges();


            var entityExpense = GetExpenseByID(newExpenseInDB.Id);
            var expenseResponse = MapExpenseResponseFromEntity(entityExpense);
            response.Add(expenseResponse);
            return response;

        }

        public BaseResponse DeleteAllExpenses()
        {
            var isAuthorizedForOperation = accessor.HttpContext.Session.GetString("hasAuthorizationToDeleteAll") == "true";

            if (!isAuthorizedForOperation)
                throw new Exception("Token no válido para ésta operación, favor de generar un token especial con usuario administrador");

            var companyID = Guid.Parse(accessor.HttpContext.Session.GetString("companyID"));

            var localDbContext = new OpenERP_RVContext();
            var allExpenses = localDbContext.Expenses.Include(i => i.ExpenseItems).Where(w => w.CompanyId == companyID);


            foreach (var expense in allExpenses)
            {
                //localDbContext.ExpenseItems.RemoveRange(expense.ExpenseItems.ToList());
                var expenseItems = expense.ExpenseItems.ToList();
                foreach (var ei in expenseItems)
                {
                    localDbContext.Remove(ei);
                }
                localDbContext.Expenses.Remove(expense);

            }
            localDbContext.SaveChanges();

            return new BaseResponse();

        }

        public BaseResponse DeleteExpenseByID(Guid id)
        {
            var deleteResponse = new BaseResponse();
            try
            {
                var expense = GetExpenseByID(id);
                if (expense == null)
                    throw new Exception("No se encotró el gasto solicitado o pertenece a otra empresa");

                DbContext.ExpenseItems.RemoveRange(expense.ExpenseItems);
                DbContext.SaveChanges();

                DbContext.Expenses.Remove(expense);
                DbContext.SaveChanges();

                deleteResponse.ErrorMessages = new List<string>();
                deleteResponse.AdditionalInformation = "Gasto removido con éxito";
            }
            catch (Exception ex)
            {
                deleteResponse.ErrorMessages.Add(ex.Message + " : " + ex.InnerException != null ? ex.InnerException.Message : "");
            }

            return deleteResponse;
        }

        public ExpenseModel GetExpenseById(Guid id)
        {

            Expense expense = GetExpenseByID(id);
            return MapExpenseResponseFromEntity(expense, includeXML: true);

        }

        private Expense GetExpenseByID(Guid id)
        {
            var companyID = Guid.Parse(accessor.HttpContext.Session.GetString("companyID"));
            return GetCompanyExpenses().Include(i => i.Supplier).Include(i2 => i2.ExpenseItems).FirstOrDefault(f => f.CompanyId == companyID && f.Id == id);
        }

        public PagedListModel<ExpenseModel> GetAllExpenses(int currentPage = 0, int itemsPerPage = 10, string searchTerm = "",
            DateTime? emissionStartDate = null, DateTime? emissionEndDate = null,
             DateTime? creationStartDate = null, DateTime? creationEndDate = null)
        {
            IOrderedQueryable<Expense> queryableData = GetFilteredExpenseData(searchTerm, emissionStartDate, emissionEndDate);

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

        public IOrderedQueryable<Expense> GetFilteredExpenseData(string searchTerm, DateTime? emissionStartDate, DateTime? emissionEndDate)
        {
            var queryableData = GetCompanyExpenses().OrderByDescending(o => o.ExpenseDate);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                queryableData = (IOrderedQueryable<Expense>)queryableData
                    .Where(w => w.Supplier.CompanyName.Contains(searchTerm)
                     || w.SupplierRfc.Contains(searchTerm)
                     || w.Supplier.Email.Contains(searchTerm));

            if (emissionStartDate.HasValue)
                queryableData = (IOrderedQueryable<Expense>)queryableData.Where(w => w.ExpenseDate > emissionStartDate.Value);

            if (emissionEndDate.HasValue)
            {
                emissionEndDate = emissionEndDate.Value.AddDays(1);
                queryableData = (IOrderedQueryable<Expense>)queryableData.Where(w => w.ExpenseDate <= emissionEndDate.Value);
            }

            return queryableData;
        }

        public List<ExpenseItemCSV> GetAllExpenseItems()
        {
            var expensesItems = DbContext.ExpenseItems.Where(w => w.Expense.CompanyId == Guid.Parse(accessor.HttpContext.Session.GetString("companyID")))
                .Include(i => i.Expense).ThenInclude(ti => ti.Supplier).OrderBy(o => o.Expense.ExpenseDate).Select(s => new ExpenseItemCSV
                {

                    Description = s.Description.Replace(",", "-"),
                    Total = s.Importe + (decimal)(s.TotalTaxes.HasValue ? s.TotalTaxes : 0m),
                    Subtotal = s.UnitPrice * s.Quantity,
                    FullFilled = s.FullFilled.Value ? "OK" : "PENDIENTE",
                    ProviderName = s.Expense.Supplier.CompanyName,
                    ProviderRFC = s.Expense.SupplierRfc,
                    ExpenseDate = s.Expense.ExpenseDate,
                    ExpenseID = s.ExpenseId,
                    Tax = s.TotalTaxes,
                    HasCFDI = !string.IsNullOrWhiteSpace(s.Expense.Xml),

                }).ToList();

            return expensesItems;
        }


        #region helpers

        public IQueryable<Expense> GetCompanyExpenses(Guid? companyID = null)
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
                XML = !string.IsNullOrWhiteSpace(expense.Xml) && includeXML ? expense.Xml : null,
                Uuid = expense.Uuid.ToString(),
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
                        ExpenseID = s.ExpenseId,
                        FullFilled = s.FullFilled.Value
                    }).ToList(),


            };
        }

        #endregion
    }
}
