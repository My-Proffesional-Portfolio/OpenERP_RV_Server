using Microsoft.AspNetCore.Http;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Models.PagedModels;
using OpenERP_RV_Server.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class ProductService : BaseService
    {
        public PagedListModel<ProductModel> GetAllProducts(int currentPage = 0, int itemsPerPage = 10)
        {
            var queryableData = GetCorporateOfficeProducts().OrderBy(o => o.Number);
            var pagedProducts = queryableData.GetPagedData(currentPage, itemsPerPage);

            var products = pagedProducts.Select(s => new ProductModel
            {
                Id = s.Id,
                Name = s.ProductName,
                BarCode = s.BarCode,
                Cost = s.Cost,
                Description = s.Description,
                InternalCode = s.InternalCode,
                Number = s.Number,
                Price = s.Price,
            }).ToList();

            return UtilService.GetPagedEntityModel(itemsPerPage, queryableData, products);
        }


        public IQueryable<Product> GetCorporateOfficeProducts(Guid? corporateOfficeId = null)
        {
            if (corporateOfficeId == null)
            {
                corporateOfficeId = Guid.Parse(HttpContext.Session.GetString("corporateOfficeID"));
            }

            var products = DbContext.Products.Where(w => w.CorporateOfficeId == corporateOfficeId.Value);
            return products;
        }
    }
}
