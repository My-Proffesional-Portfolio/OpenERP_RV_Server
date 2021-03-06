using Microsoft.IdentityModel.Tokens;
using OpenERP_RV_Server.DataAccess;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class SecurityService : BaseService
    {
        public static string EncryptPassword(string password, string saltValue)
        {

            string secretKey = UtilService.GetAppSettingsConfiguration("security", "passwordPrivateKey");
            var saltBuffer = Encoding.UTF8.GetBytes(saltValue);
            byte[] clearBytes = Encoding.Unicode.GetBytes(password);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(secretKey, saltBuffer, 1000, HashAlgorithmName.SHA256);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    password = Convert.ToBase64String(ms.ToArray());
                }
            }

            return password;
        }

        public static string DecryptPassword(string cipherText, string saltValue)
        {
            string secretKey = UtilService.GetAppSettingsConfiguration("security", "passwordPrivateKey");
            var saltBuffer = Encoding.UTF8.GetBytes(saltValue);
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(secretKey, saltBuffer, 1000, HashAlgorithmName.SHA256);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public string GenerateJSONWebToken(User userInfo, ref DateTime expiration, bool generateSpecialToken = false)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(UtilService.GetAppSettingsConfiguration("security", "JWT_PrivateKey")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("isValidPublicAPI_Token", "true"));
            permClaims.Add(new Claim("userId", userInfo.UserId.ToString()));
            permClaims.Add(new Claim("name", userInfo.UserName));
            permClaims.Add(new Claim("companyId", userInfo.CompanyId.ToString()));
            permClaims.Add(new Claim("email", userInfo.Email));
            if (userInfo.IsAdmin && generateSpecialToken)
                permClaims.Add(new Claim("specialAuthorization", "true"));


            var token = new JwtSecurityToken(UtilService.GetAppSettingsConfiguration("security", "issuer"),
              UtilService.GetAppSettingsConfiguration("security", "audicence"), claims: permClaims,
              null,
              expires: DateTime.Now.AddDays(365),
              signingCredentials: credentials);

            //TODO: get time from token object
            expiration = !generateSpecialToken ? DateTime.Now.AddMinutes(60) : DateTime.Now.AddSeconds(30);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
