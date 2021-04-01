using Microsoft.EntityFrameworkCore;
using OpenERP_RV_Server.DataAccess;
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

            using var context = DbContext;
            using var transaction = context.Database.BeginTransaction();
            var result = new NewCompanyOrganizationResult();
            try
            {
                var newCorporateOffice = AddNewCompanyCorporateOffice(company);
                var newCompany = AddNewCompany(company, newCorporateOffice.Id, company.Address);
                var user = new UserService().AddNewUser(company.UserName, company.Password, company.Phone, company.Email, newCompany.Id, newCorporateOffice.Id);

                result = new NewCompanyOrganizationResult();
                result.LegalName = newCompany.LegalName;
                result.UserName = user.UserName;
                result.CorporateOfficeNumber = newCorporateOffice.CorporativeOfficeNumber;
                var tokenExpiration = DateTime.Now;
                result.UserToken = new SecurityService().GenerateJSONWebToken(user, ref tokenExpiration);
                result.UserTokenExpiration = tokenExpiration;

                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.Message);
                return result;
            }
        }

        private CorporateOffice AddNewCompanyCorporateOffice(NewCompanyOrganizationModel company)
        {
            var newCorporateOffice = new CorporateOffice();
            newCorporateOffice.Address = company.Address;
            newCorporateOffice.ContactName = company.ContactName;
            newCorporateOffice.Id = Guid.NewGuid();
            newCorporateOffice.CorporativeOfficeNumber = GetCorporativeOfficeNumber();
            newCorporateOffice.Name = company.LegalName;
            DbContext.CorporateOffices.Add(newCorporateOffice);
            DbContext.SaveChanges();
            return newCorporateOffice;
        }

        private Company AddNewCompany(NewCompanyOrganizationModel company, Guid newCorporateOfficeId, string address)
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
            DbContext.SaveChanges();
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
            return DbContext.CorporateOffices.AsQueryable();
        }

        private IQueryable<Company> GetCompanies()
        {
            return DbContext.Companies.Include(i=> i.CorporateOffice).AsQueryable();
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
    }
}
