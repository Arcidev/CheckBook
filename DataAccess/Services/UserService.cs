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
                var user = db.Users.FirstOrDefault(x => x.Email == email);
                if (user == null)
                    return null;

                return ToUserData(user);
            }
        }

        public static CreateUserResult CreateUser(UserData user)
        {
            if (!user.HasValidData)
                return CreateUserResult.CannotCreate;

            using (var db = new AppContext())
            {
                var dbUser = db.Users.FirstOrDefault(x => x.Email == user.Email);
                if (dbUser != null)
                    return CreateUserResult.UserAlreadyExists;

                var passwordData = PasswordHelper.CreateHash(user.Password);

                db.Users.Add(new User()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PasswordSalt = passwordData.PasswordSalt,
                    PasswordHash = passwordData.PasswordHash
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

        private static UserData ToUserData(User user)
        {
            return new UserData()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt
            };
        }

        private static UserInfoData ToUserInfoData(User user)
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
