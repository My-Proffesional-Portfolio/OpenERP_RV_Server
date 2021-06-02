using Microsoft.AspNetCore.Http;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Models.PagedModels;
using OpenERP_RV_Server.Models.SalesConcept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class SalesConceptService : BaseService
    {
        public IQueryable<SalesConcept> GetCorporateOfficeSalesConcepts(Guid? corporateOfficeId = null)
        {
            if (corporateOfficeId == null)
            {
                corporateOfficeId = Guid.Parse(accessor.HttpContext.Session.GetString("corporateOfficeID"));
            }
            var concepts = DbContext.SalesConcepts.Where(w => w.CorporateOfficeId == corporateOfficeId.Value);
            return concepts;
        }

        public PagedListModel<ConceptsModel> GetPagedClients(int currentPage = 0, int itemsPerPage = 10)
        {
            var queryableData = GetCorporateOfficeSalesConcepts().OrderBy(o => o.Number);

            var pagedConcepts = queryableData.GetPagedData(currentPage, itemsPerPage);
            var concepts = pagedConcepts.Select(s => new ConceptsModel
            {
                Id = s.Id,
                Name = s.ServiceName,
                Cost = s.Cost,
                Description = s.Description,
                InternalCode = s.InternalCode,
                Number = s.Number,
                Price = s.Price,
            }).ToList();

            return UtilService.GetPagedEntityModel(itemsPerPage, queryableData, concepts);
        }
    }
}
