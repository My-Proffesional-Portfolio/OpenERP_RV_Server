﻿using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Models;
using OpenERP_RV_Server.Models.Client.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class ClientService : BaseService
    {
        //public 
        public ClientResponseModel AddNewClient(ClientModel clientModel, OpenERP_RVContext trasnsactionContext = null)
        {
            var newClient = new Client();
            newClient.CorporateOfficeId = clientModel.CorporateOfficeId;
            newClient.Id = Guid.NewGuid();
            newClient.Number = GetNextClientNumber(clientModel.CorporateOfficeId);
            newClient.CompanyName = clientModel.CompanyName;
            newClient.LegalName = clientModel.LegalName;
            newClient.ContactName = clientModel.ContactName;
            newClient.FiscalIdentifier = clientModel.FiscalTaxID;
            newClient.DeliveryAddress = clientModel.DeliveryAddress;
            newClient.BusinessCategoryId = clientModel.BusinessCategoryID.HasValue ? clientModel.BusinessCategoryID.Value
                : DbContext.BusinessCategories.FirstOrDefault(f => f.Description == "No especificado").Id;
            newClient.ClientCompanyStatusId = clientModel.ClientCompanyStatusId;

            DbContext.Clients.Add(newClient);
            //DbContext.Clients.Add(newClient);

            //DbContext.SaveChanges();

            return new ClientResponseModel()
            {
                Id = newClient.Id,
                CorporateOfficeID = newClient.CorporateOfficeId,
                ClientNumber = newClient.Number
            };

        }

        public IQueryable<Client> GetCorporateOfficeClients(Guid corporateOfficeId)
        {
            var clients = DbContext.Clients.Where(w => w.CorporateOfficeId == corporateOfficeId);
            return clients;
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