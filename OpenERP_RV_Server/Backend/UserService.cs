using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.ExceptionTypes;
using OpenERP_RV_Server.Models.Account.Request;
using OpenERP_RV_Server.Models.Account.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
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

        public User AddNewUser(string userName, string password, string phone, string email, Guid newCompanyID, Guid? newCorporateOfficeID)
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
            //DbContext.Users.Add(newUser);
            DbContext.Users.Add(newUser);
            // DbContext.SaveChanges();
            return newUser;
        }

        public LoginResponseModel Login(LoginModel login)
        {
            var selectedUser = GetUserByName(login.UserName);
            var response = new LoginResponseModel();
            if (selectedUser == null)
            {
                //response.ErrorMessages.Add("El usuario solicitado no existe, revisar nombre de usuario");
                //return response;
                throw new FriendlyNotFoundException("No se pudo encontrar el usuario especificado, favor de revisar el nombre de usuario");
            }
            var salt = selectedUser.Salt;
            var hashedPassword = selectedUser.HashedPassword;
            var decryptedPassword = SecurityService.DecryptPassword(hashedPassword, salt);
            var tokenLimitDate = DateTime.Now;


            if (decryptedPassword == login.Password)
            {
                var token = new SecurityService().GenerateJSONWebToken(selectedUser, ref tokenLimitDate, login.SpecialRequestToken);
                response.Token = token;
                response.TokenExpiration = tokenLimitDate;
                response.CompanyID = selectedUser.CompanyId.ToString();

                var company = new CompanyOrganizationService().GetCompanyByID(selectedUser.CompanyId);
                var corporate = new CompanyOrganizationService().GetCorporateByCompanyID(selectedUser.CompanyId);
                response.CorporateOfficeID = corporate.Id.ToString();
                response.CompanyLegalName = company.LegalName;
                response.UserName = selectedUser.UserName;

                //https://stackoverflow.com/questions/39920954/asp-core-how-to-set-httpcontext-user
                var user = new GenericPrincipal(new ClaimsIdentity(response.UserName), null);
                accessor.HttpContext.User = user;

                accessor.HttpContext.Session.SetString("companyID", response.CompanyID);
                accessor.HttpContext.Session.SetString("corporateOfficeID", response.CorporateOfficeID);
                accessor.HttpContext.Session.SetString("userName", response.UserName);
                accessor.HttpContext.Session.SetString("token", token);

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

        public LoginSessionData GetCurrentUserSession()
        {
            var response = new LoginSessionData();
            response.CurrentUserName = accessor.HttpContext.Session.GetString("userName");
            response.CurrentToken = accessor.HttpContext.Session.GetString("token");
            response.CompanyLogged = accessor.HttpContext.Session.GetString("companyID");

            return response;
        }

        public void MockUserSessionByToken(ActionExecutingContext context)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //if (!string.IsNullOrEmpty(accessor.HttpContext.Session.GetString("corporateOfficeID")))
            //    return;

            string tokenBearer = context.HttpContext.Request.Headers["Authorization"];
            var token = tokenBearer.Split(new string[] { "Bearer " }, StringSplitOptions.None)[1];

            var stream = token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;

            var companyID = tokenS.Claims.First(claim => claim.Type == "companyId").Value;
            var userName = tokenS.Claims.First(claim => claim.Type == "name").Value;
            var securityAuthorization = tokenS.Claims.FirstOrDefault(claim => claim.Type == "specialAuthorization")?.Value;


            var user = new GenericPrincipal(new ClaimsIdentity(userName), null);
            accessor.HttpContext.User = user;

            accessor.HttpContext.Session.SetString("companyID", companyID);
            var corporateOfficeID = new CompanyOrganizationService().GetCorporateByCompanyID(Guid.Parse(companyID)).Id;
            accessor.HttpContext.Session.SetString("corporateOfficeID", corporateOfficeID.ToString());
            accessor.HttpContext.Session.SetString("userName", userName);
            accessor.HttpContext.Session.SetString("token", token);
            if (!string.IsNullOrWhiteSpace(securityAuthorization))
                accessor.HttpContext.Session.SetString("hasAuthorizationToDeleteAll", securityAuthorization);

            BaseService.corporateOfficeID = corporateOfficeID.ToString();
            BaseService.companyID = companyID.ToString();
            BaseService.userName = userName;


            sw.Stop();

            var elapsetMiliseconds = sw.ElapsedMilliseconds;
        }
    }
}
