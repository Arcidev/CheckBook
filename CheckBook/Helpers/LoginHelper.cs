using CheckBook.Models;
using System.Security.Claims;

namespace CheckBook.Helpers
{
    public static class LoginHelper
    {
        public static ClaimsIdentity GetClaimsIdentity(string email, string password)
        {
            var user = DataAccess.DbAccess.GetUser(email);
            if (user == null)
                return null;

            if (user.Password != password)
                return null;

            var claimsIdentity = new ClaimsIdentity(new UserIdentity(user.FullName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            return claimsIdentity;
        }
    }
}