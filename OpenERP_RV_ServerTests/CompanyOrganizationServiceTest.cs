using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenERP_RV_Server.Backend;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Models.CompanyOrganization;
using System;
using System.Linq;

namespace OpenERP_RV_ServerTests
{
    [TestClass]
    public class CompanyOrganizationServiceTest
    {


        Mock<IDbContextTransaction> txn;
        Mock<OpenERP_RVContext> dbContext;

        [TestMethod]
        public void TestMethod1()
        {
            //_companyService.Setup(s => s.AddNewCompanyOrganization(It.IsAny<NewCompanyOrganizationModel>())).Returns(new NewCompanyOrganizationResult());
            //_companyService.Object.AddNewCompanyOrganization(new NewCompanyOrganizationModel());
            txn = new Mock<IDbContextTransaction>();
            dbContext = new Mock<OpenERP_RVContext>();

            txn.Setup(x => x.Commit()).Verifiable();
            dbContext.Setup(x => x.SaveChanges()).Verifiable();
            dbContext.Setup(x => x.CorporateOffices.Add(It.IsAny<CorporateOffice>())).Verifiable();
            dbContext.Setup(x => x.Companies.Add(It.IsAny<Company>())).Verifiable();
            dbContext.Setup(x => x.Users.Add(It.IsAny<User>())).Verifiable();

            Mock<CompanyOrganizationService> _companyService = new Mock<CompanyOrganizationService>(txn.Object, dbContext.Object) { CallBase = true };
            Mock<UserService> _userService = new Mock<UserService>(dbContext.Object);

            _companyService.Setup(x => x.GetDefaultBussinessCategoryID()).Returns(Guid.NewGuid);
            _userService.Setup(x => x.GetUserByName(It.IsAny<string>())).Returns(()=> null);


            _companyService.Setup(x => x.GetCorporativeOfficeNumber()).Returns(1000);
            _companyService.Object.AddNewCompanyOrganization(new NewCompanyOrganizationModel() { CommercialName = "test", ContactName = "test", UserName = "Test", Password= "123456" });
        }
    }
}
