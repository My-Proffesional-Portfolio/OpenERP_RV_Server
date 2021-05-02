using Microsoft.AspNetCore.Http;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Models;
using OpenERP_RV_Server.Models.Client.Response;
using OpenERP_RV_Server.Models.PagedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class ClientService : BaseService
    {
        //public 
        public ClientResponseModel AddNewClient(ClientModel clientModel)
        {
            var newClient = new Client();
            newClient.CorporateOfficeId = Guid.Parse(HttpContext.Session.GetString("corporateOfficeID"));
            newClient.Id = Guid.NewGuid();
            newClient.Number = GetNextClientNumber(newClient.CorporateOfficeId);
            newClient.CompanyName = clientModel.CompanyName;
            newClient.LegalName = clientModel.LegalName;
            newClient.ContactName = clientModel.ContactName;
            newClient.FiscalIdentifier = clientModel.FiscalTaxID;
            newClient.DeliveryAddress = clientModel.DeliveryAddress;
            newClient.Email = clientModel.Email;
            newClient.Phone = clientModel.Phone;
            newClient.FiscalAddress = clientModel.FiscalAddress;
            newClient.BusinessCategoryId = clientModel.BusinessCategoryID.HasValue ? clientModel.BusinessCategoryID.Value
                : DbContext.BusinessCategories.FirstOrDefault(f => f.Description == "No especificado").Id;
            newClient.ClientCompanyStatusId = clientModel.ClientCompanyStatusId;

            DbContext.Clients.Add(newClient);

            DbContext.SaveChanges();

            return new ClientResponseModel()
            {
                Id = newClient.Id,
                CorporateOfficeID = newClient.CorporateOfficeId,
                ClientNumber = newClient.Number
            };

        }

        public IQueryable<Client> GetCorporateOfficeClients(Guid? corporateOfficeId = null)
        {
            if (corporateOfficeId == null)
            {
                corporateOfficeId = Guid.Parse(HttpContext.Session.GetString("corporateOfficeID"));
            }
            var clients = DbContext.Clients.Where(w => w.CorporateOfficeId == corporateOfficeId.Value);
            return clients;
        }

        public PagedListModel<ClientModel> GetPagedClients(int currentPage = 0, int itemsPerPage = 10)
        {
            var queryableData = GetCorporateOfficeClients().OrderBy(o=> o.Number);

            var pagedClients = queryableData.GetPagedData(currentPage, itemsPerPage);
            var clients = pagedClients.Select(s => new ClientModel
            {
                Id = s.Id,
                CompanyName = s.CompanyName,
                DeliveryAddress = s.DeliveryAddress,
                FiscalTaxID = s.FiscalIdentifier,
                LegalName = s.LegalName,
                ContactName = s.ContactName,
                CorporateOfficeId = s.CorporateOfficeId,
                Number = s.Number,
                FiscalAddress = s.FiscalAddress,
                Phone = s.Phone,
                Email = s.Email

            }).ToList();
            return UtilService.GetPagedEntityModel(itemsPerPage, queryableData, clients);
        }

        private long GetNextClientNumber(Guid corporateOfficeId)
        {
            var corporateClients = GetCorporateOfficeClients(corporateOfficeId);
            if (corporateClients.Count() == 0)
                return 1000;
            else
                return corporateClients.OrderByDescending(o => o.Number).FirstOrDefault().Number + 1;
        }
    }
}
