using CheckBook.Models;
using DataAccess.Services;
using System.Security.Claims;

namespace CheckBook.Helpers
{
    public static class LoginHelper
    {
        public static ClaimsIdentity GetClaimsIdentity(string email, string password)
        {
            var user = UserService.GetUser(email);
            if (user == null)
                return null;

            if (!DataAccess.Security.PasswordHelper.VerifyHashedPassword(user.PasswordHash, user.PasswordSalt, password))
                return null;

            var claimsIdentity = new ClaimsIdentity(new UserIdentity(user.FullName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            return claimsIdentity;
        }
    }
}