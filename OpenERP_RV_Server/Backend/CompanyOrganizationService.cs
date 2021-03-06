using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.ExceptionTypes;
using OpenERP_RV_Server.Models;
using OpenERP_RV_Server.Models.Client.Response;
using OpenERP_RV_Server.Models.CompanyOrganization;
using OpenERP_RV_Server.Models.SalesConcept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class CompanyOrganizationService : BaseService
    {
        private IDbContextTransaction _transaction;
        public CompanyOrganizationService()
        {
           _transaction = DbContext.Database.BeginTransaction();
        }

        public CompanyOrganizationService(IDbContextTransaction transaction, OpenERP_RVContext dbContext)
        {
            _transaction = transaction;
            DbContext = dbContext;
        }
        public virtual NewCompanyOrganizationResult AddNewCompanyOrganization(NewCompanyOrganizationModel company)
        {
            //using var context = new OpenERP_RVContext();
            

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
                AddGenericConcept(new ConceptsModel { 
                    Cost = null,
                    Price = 0m,
                    InternalCode  = "OPEN_ERP_GENERIC_SERVICE",
                    Number = 1000,
                    Description = "CONCEPTO GENÉRICO DE VENTA",
                    Id = Guid.NewGuid(),
                    Name = "Concepto de venta genérico.",
                    CorporateOfficeId = newCorporateOffice.Id,
                   
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
                _transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                throw new FriendlyTransactionException("No se pudo completar la transacción: " + ex.Message);
            }
        }

        private void AddGenericConcept(ConceptsModel conceptsModel)
        {
            var newService = new SalesConcept();
            newService.Id = conceptsModel.Id;
            newService.CorporateOfficeId = conceptsModel.CorporateOfficeId;
            newService.Cost = conceptsModel.Cost;
            newService.Price = conceptsModel.Price;
            newService.ServiceName = conceptsModel.Name;
            newService.Number = conceptsModel.Number;
            newService.CreationDate = DateTime.Now;
            newService.Description = conceptsModel.Description;
            newService.InternalCode = conceptsModel.InternalCode;
            DbContext.SalesConcepts.Add(newService);

        }

        private ConfirmationResponseModel AddNewCorporativeDefaultClient(ClientModel clientModel)
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

            return new ConfirmationResponseModel()
            {
                Id = newClient.Id,
                CorporateOfficeID = newClient.CorporateOfficeId,
                Number = newClient.Number
            };

        }

        private User AddNewAdminCompanyUser(string userName, string password, string phone, string email, Guid newCompanyID, Guid newCorporateOfficeID)
        {
            var newUser = new User();
            newUser.UserId = Guid.NewGuid();
            newUser.CompanyId = newCompanyID;

            if (new UserService().GetUserByName(userName) != null)
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
            newCompany.BusinessCategoryId = GetDefaultBussinessCategoryID();
            DbContext.Companies.Add(newCompany);
            //DbContext.Companies.Add(newCompany);
            //DbContext.SaveChanges();
            return newCompany;
        }

        public virtual Guid GetDefaultBussinessCategoryID()
        {
           return DbContext.BusinessCategories.FirstOrDefault(f => f.Description == "No especificado").Id;
        }

        public virtual long GetCorporativeOfficeNumber()
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