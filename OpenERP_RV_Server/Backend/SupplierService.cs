using Microsoft.AspNetCore.Http;
using OpenERP_RV_Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class SupplierService : BaseService
    {
        public object GetAllSuppliers()
        {
            var companySuppliers = GetCompanySuppliers().OrderBy(o=> o.CompanyName);
            var response = companySuppliers.Select(s => new { s.Rfc, s.CompanyName, s.Id, s.AddressLocation }).ToList();
            return response;
        }


        public IQueryable<Supplier> GetCompanySuppliers(Guid? companyID = null)
        {
            if (companyID == null)
            {
                companyID = Guid.Parse(accessor.HttpContext.Session.GetString("companyID"));
            }

            var suppliers = DbContext.Suppliers.Where(w => w.CompanyId == companyID.Value);
            return suppliers;
        }
    }
}
