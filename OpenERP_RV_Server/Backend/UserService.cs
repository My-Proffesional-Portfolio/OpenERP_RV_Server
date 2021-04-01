using Microsoft.AspNetCore.Http;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Models.Account.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class UserService : BaseService
    {

        public IQueryable<User> GetUsers() 
        {
            return DbContext.Users.AsQueryable();
        }

        public User AddNewUser(string userName, string password, string phone, string email, Guid newCompanyID, Guid? newCorporateOfficeID )
        {
            var newUser = new User();
            newUser.UserId = Guid.NewGuid();
            newUser.CompanyId = newCompanyID;

            if (GetUsers().FirstOrDefault(f => f.UserName == userName) != null)
                throw new Exception("El usuario ingresado ya existe en nuestra base de datos, intenta otro nombre de usuario");

            newUser.UserName = userName;
            newUser.Salt = Guid.NewGuid().ToString();
            newUser.HashedPassword = SecurityService.EncryptPassword(password, newUser.Salt);
            newUser.Phone = phone;
            newUser.Status = true;
            newUser.Email = email;
            newUser.IsAdmin = newCorporateOfficeID.HasValue;
            DbContext.Users.Add(newUser);
            DbContext.SaveChanges();
            return newUser;
        }

        public LoginResponseModel Login(LoginModel login)
        {
            var selectedUser = GetUserByName(login.UserName);
            var response = new LoginResponseModel();
            if (selectedUser == null)
            {
                response.ErrorMessages.Add("El usuario solicitado no existe, revisar nombre de usuario");
            }
            var salt = selectedUser.Salt;
            var hashedPassword = selectedUser.HashedPassword;
            var decryptedPassword = SecurityService.DecryptPassword(hashedPassword, salt);
            var tokenLimitDate = DateTime.Now;


            if (decryptedPassword == login.Password)
            {
                var token = new SecurityService().GenerateJSONWebToken(selectedUser, ref tokenLimitDate);
                response.Token = token;
                response.TokenExpiration = tokenLimitDate;
                response.CompanyID = selectedUser.CompanyId.ToString();

                var company = new CompanyOrganizationService().GetCompanyByID(selectedUser.CompanyId);
                var corporate = new CompanyOrganizationService().GetCorporateByCompanyID(selectedUser.CompanyId);
                response.CorporateOfficeID = corporate.Id.ToString();
                response.CompanyLegalName = company.LegalName;
                response.UserName = selectedUser.UserName;

                HttpContext.Session.SetString("companyID", response.CompanyID);
                HttpContext.Session.SetString("userName", response.UserName);

            }
            else
            {
                response.ErrorMessages.Add("El password no coincide con el usuario");
            }


            return response;

        }

        private User GetUserByName(string userName)
        {
            return GetUsers().Where(w => w.UserName == userName).FirstOrDefault(); ;
        }

    }
}
