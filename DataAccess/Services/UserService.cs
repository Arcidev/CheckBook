using DataAccess.Context;
using DataAccess.Data;
using DataAccess.Enums;
using DataAccess.Model;
using DataAccess.Security;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Services
{
    public static class UserService
    {
        public static UserData GetUser(string email)
        {
            using (var db = new AppContext())
            {
                email = email.Trim().ToLower();
                var user = db.Users.FirstOrDefault(x => x.Email == email);
                if (user == null)
                    return null;

                return ToUserData(user);
            }
        }

        public static CreateUserResult CreateUser(UserData user)
        {
            using (var db = new AppContext())
            {
                var email = user.Email.Trim().ToLower();
                var dbUser = db.Users.FirstOrDefault(x => x.Email == email);
                if (dbUser != null)
                    return CreateUserResult.UserAlreadyExists;

                var passwordData = PasswordHelper.CreateHash(user.Password);

                db.Users.Add(new User()
                {
                    Email = email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PasswordSalt = passwordData.PasswordSalt,
                    PasswordHash = passwordData.PasswordHash,
                    UserRole = user.UserRole
                });

                db.SaveChanges();
                return CreateUserResult.Success;
            }
        }

        public static List<UserInfoData> GetUsersInfo()
        {
            using (var db = new AppContext())
            {
                return db.Users.Select(ToUserInfoData).ToList();
            }
        }

        public static UserData ToUserData(User user)
        {
            return new UserData()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
                UserRole = user.UserRole
            };
        }

        public static UserInfoData ToUserInfoData(User user)
        {
            return new UserInfoData()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
