using OktaSimpleAuth.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OktaSimpleAuth.Services
{
    public class UserService
    {
        private readonly AuthDbContext _db;
        public UserService(AuthDbContext db)
        {
            _db = db;
        }

        internal AppUser GetUserByExternalProvider(string provider, string nameIdentifier)
        {
            var appUser = _db.AppUsers
                .Where(a => a.Provider == provider)
                .Where(a => a.NameIdentifier == nameIdentifier).FirstOrDefault();
            return appUser;
        }

        internal AppUser GetUserById(int id)
        {
            var appUser = _db.AppUsers.Find(id);
            return appUser;
        }

        internal bool TryValidateUser(string username, string password, out List<Claim> claims)
        {
            claims = new List<Claim>();
            var appUser = _db.AppUsers
                .Where(a => a.Username == username)
                .Where(a => a.Password == password).FirstOrDefault();
            if (appUser is null)
            {
                return false;
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, username));
                claims.Add(new Claim("UserName" , username));
                claims.Add(new Claim(ClaimTypes.GivenName, appUser.Firstname));
                claims.Add(new Claim(ClaimTypes.Surname, appUser.Lastname));
                claims.Add(new Claim(ClaimTypes.Email, appUser.Email));
                claims.Add(new Claim(ClaimTypes.MobilePhone, appUser.Mobile));
                foreach (var r in appUser.RoleList)
                {
                    claims.Add(new Claim(ClaimTypes.Role, r));
                }
                return true;
            }
        }

        internal AppUser RegisterNewUser(string provider, List<Claim> claims)
        {
            var appUser = new AppUser();
            appUser.Provider = provider;
            appUser.NameIdentifier = claims.GetClaim(ClaimTypes.NameIdentifier);
            appUser.Username = claims.GetClaim("username");
            appUser.Firstname = claims.GetClaim(ClaimTypes.GivenName);
            appUser.Lastname = claims.GetClaim(ClaimTypes.Surname);
            if (string.IsNullOrEmpty(appUser.Firstname))
            {
                appUser.Firstname = claims.GetClaim("name");
            }
            appUser.Email = claims.GetClaim(ClaimTypes.Email);
            appUser.Mobile = claims.GetClaim(ClaimTypes.MobilePhone);
            var entity = _db.AppUsers.Add(appUser);
            _db.SaveChanges();
            return entity.Entity;
        }
    }

    public static class Extensions
    {
        public static string GetClaim(this List<Claim> claims, string name)
        {
            return claims.FirstOrDefault(c => c.Type == name)?.Value;
        }
    }
}
