using Microsoft.EntityFrameworkCore;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.ExceptionTypes;
using OpenERP_RV_Server.Models;
using OpenERP_RV_Server.Models.Client.Response;
using OpenERP_RV_Server.Models.CompanyOrganization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class CompanyOrganizationService : BaseService
    {
        public NewCompanyOrganizationResult AddNewCompanyOrganization(NewCompanyOrganizationModel company)
        {
            //using var context = new OpenERP_RVContext();
            using var transaction = DbContext.Database.BeginTransaction();

            var result = new NewCompanyOrganizationResult();
            try
            {
                //Methods are not in another service because DbContext scope in transaction :c
                var newCorporateOffice = AddNewCompanyCorporateOffice(company);
                var newCompany = AddNewCompany(company, newCorporateOffice.Id, company.Address);
                var user = AddNewAdminCompanyUser(company.UserName, company.Password, company.Phone, company.Email, newCompany.Id, newCorporateOffice.Id);
                AddNewCorporativeDefaultClient(new ClientModel()
                {
                    CompanyName = "Público en general",
                    ContactName = "Público en general",
                    CorporateOfficeId = newCorporateOffice.Id,
                    FiscalTaxID = "XAXX010101000"
                });

                result = new NewCompanyOrganizationResult();
                result.LegalName = newCompany.LegalName;
                result.UserName = user.UserName;
                result.CorporateOfficeNumber = newCorporateOffice.CorporativeOfficeNumber;
                var tokenExpiration = DateTime.Now;
                result.UserToken = new SecurityService().GenerateJSONWebToken(user, ref tokenExpiration);
                result.UserTokenExpiration = tokenExpiration;

                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                DbContext.SaveChanges();
                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                throw new FriendlyTransactionException("No se pudo completar la transacción: " + ex.Message);
            }
        }

        private ClientResponseModel AddNewCorporativeDefaultClient(ClientModel clientModel)
        {
            var newClient = new Client();
            newClient.CorporateOfficeId = clientModel.CorporateOfficeId;
            newClient.Id = Guid.NewGuid();
            newClient.Number = 1000;
            newClient.CompanyName = clientModel.CompanyName;
            newClient.LegalName = clientModel.LegalName;
            newClient.ContactName = clientModel.ContactName;
            newClient.FiscalIdentifier = clientModel.FiscalTaxID;
            newClient.DeliveryAddress = clientModel.DeliveryAddress;
            newClient.BusinessCategoryId = DbContext.BusinessCategories.FirstOrDefault(f => f.Description == "No especificado").Id;
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

        private User AddNewAdminCompanyUser(string userName, string password, string phone, string email, Guid newCompanyID, Guid newCorporateOfficeID)
        {
            var newUser = new User();
            newUser.UserId = Guid.NewGuid();
            newUser.CompanyId = newCompanyID;

            if (new UserService().GetUsers().FirstOrDefault(f => f.UserName == userName) != null)
                throw new Exception("El usuario ingresado ya existe en nuestra base de datos, intenta otro nombre de usuario");

            newUser.UserName = userName;
            newUser.Salt = Guid.NewGuid().ToString();
            newUser.HashedPassword = SecurityService.EncryptPassword(password, newUser.Salt);
            newUser.Phone = phone;
            newUser.Status = true;
            newUser.Email = email;
            newUser.IsAdmin = true;
            DbContext.Users.Add(newUser);
            // DbContext.SaveChanges();
            return newUser;
        }

        private CorporateOffice AddNewCompanyCorporateOffice(NewCompanyOrganizationModel company, OpenERP_RVContext trasnsactionContext = null)
        {
            
            var newCorporateOffice = new CorporateOffice();
            newCorporateOffice.Address = company.Address;
            newCorporateOffice.ContactName = company.ContactName;
            newCorporateOffice.Id = Guid.NewGuid();
            newCorporateOffice.CorporativeOfficeNumber = GetCorporativeOfficeNumber();
            newCorporateOffice.Name = company.LegalName;
            //DbContext.CorporateOffices.Add(newCorporateOffice);
            DbContext.CorporateOffices.Add(newCorporateOffice);
            //DbContext.SaveChanges();
            return newCorporateOffice;
        }

     

        private Company AddNewCompany(NewCompanyOrganizationModel company, Guid newCorporateOfficeId, string address, OpenERP_RVContext trasnsactionContext = null)
        {
            var newCompany = new Company();
            newCompany.Id = Guid.NewGuid();
            newCompany.LegalName = company.LegalName;
            newCompany.CommercialName = company.CommercialName;
            newCompany.CorporateOfficeId = newCorporateOfficeId;
            newCompany.OfficeNumberId = string.IsNullOrWhiteSpace(company.OfficeNumberId) ? Guid.NewGuid().ToString().Substring(0, 4) : company.OfficeNumberId;
            newCompany.FiscalIdentifier = company.FiscalIdentificationNumber;
            newCompany.Address = address;
            newCompany.Status = true;
            newCompany.BusinessCategoryId = DbContext.BusinessCategories.FirstOrDefault(f => f.Description == "No especificado").Id;
            DbContext.Companies.Add(newCompany);
            //DbContext.Companies.Add(newCompany);
            //DbContext.SaveChanges();
            return newCompany;
        }


        private long GetCorporativeOfficeNumber()
        {

            var lastCorporate = GetCorporates().OrderByDescending(o => o.CorporativeOfficeNumber).FirstOrDefault();
            if (lastCorporate == null)
                return 1000;

            else
                return lastCorporate.CorporativeOfficeNumber + 1;

        }

        private IQueryable<CorporateOffice> GetCorporates()
        {
            return DbContext.CorporateOffices.Include(i=> i.Companies).AsQueryable();
        }

        private IQueryable<Company> GetCompanies()
        {
            return DbContext.Companies.Include(i => i.CorporateOffice).AsQueryable();
        }

        public Company GetCompanyByID(Guid id)
        {
            return GetCompanies().FirstOrDefault(f => f.Id == id);
        }

        public CorporateOffice GetCorporateByCompanyID(Guid companyID)
        {
            var company = GetCompanyByID(companyID);
            var corporate = company.CorporateOffice;
            return corporate;
        }

        public CorporateOffice GetCorporateByCorporateID(Guid corporateID)
        {
            var corporate = GetCorporates().FirstOrDefault(f => f.Id == corporateID);
            return corporate;
        }

        public object GetCorporateInfoById(Guid corporateId)
        {
            var coporateOffice = GetCorporateByCorporateID(corporateId);

            return new
            {
                coporateOffice.Id,
                coporateOffice.Name,
                Companies = coporateOffice.Companies.Select(s => new
                {
                    s.LegalName,
                    s.CommercialName,
                    s.OfficeNumberId,
                    s.FiscalIdentifier,
                    s.Phone,
                    s.Address,
                    s.Id,
                }),
            };
        }
    }
}