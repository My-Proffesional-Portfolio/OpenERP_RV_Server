using Microsoft.AspNetCore.Http;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Models.Client.Response;
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

        private long GetNextProductNumber(Guid corporateOfficeId)
        {
            var corporateProducts = GetCorporateOfficeProducts(corporateOfficeId);
            if (corporateProducts.Count() == 0)
                return 1000;
            else
                return corporateProducts.OrderByDescending(o => o.Number).FirstOrDefault().Number + 1;
        }

        public ConfirmationResponseModel AddNewProduct(ProductModel productModel)
        {
            var newProduct = new Product();
            newProduct.CorporateOfficeId = Guid.Parse(accessor.HttpContext.Session.GetString("corporateOfficeID"));
            newProduct.Id = Guid.NewGuid();
            newProduct.Number = GetNextProductNumber(newProduct.CorporateOfficeId);
            newProduct.Price = productModel.Price;
            newProduct.BarCode = productModel.BarCode;
            newProduct.Cost = productModel.Cost;
            newProduct.Description = productModel.Description;
            newProduct.ProductName = productModel.Name;
            newProduct.InternalCode = productModel.InternalCode;
            newProduct.Id = Guid.NewGuid();
            newProduct.CreationDate = DateTime.Now;

            DbContext.Products.Add(newProduct);

            DbContext.SaveChanges();

            return new ConfirmationResponseModel()
            {
                Id = newProduct.Id,
                CorporateOfficeID = newProduct.CorporateOfficeId,
                Number = newProduct.Number
            };

        }

        public IQueryable<Product> GetCorporateOfficeProducts(Guid? corporateOfficeId = null)
        {
            if (corporateOfficeId == null)
            {
                corporateOfficeId = Guid.Parse(accessor.HttpContext.Session.GetString("corporateOfficeID"));
            }

            var products = DbContext.Products.Where(w => w.CorporateOfficeId == corporateOfficeId.Value);
            return products;
        }
    }
}
