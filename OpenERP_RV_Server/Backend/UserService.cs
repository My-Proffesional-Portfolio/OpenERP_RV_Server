using OpenERP_RV_Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
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
            newUser.ComapnyId = newCompanyID;

            if (GetUsers().FirstOrDefault(f => f.UserName == userName) != null)
                throw new Exception("El usuario ingresado ya existe en nuestra base de datos, intenta otro nombre de usuario");

            newUser.UserName = userName;
            newUser.Salt = Guid.NewGuid().ToString();
            newUser.HashedPassword = SecurityService.EncryptPassword(password, newUser.Salt);
            newUser.Phone = phone;
            newUser.Status = true;
            newUser.Email = email;
            newUser.CorporateOffice = newCorporateOfficeID;
            DbContext.Users.Add(newUser);
            DbContext.SaveChanges();
            return newUser;
        }
    }
}
