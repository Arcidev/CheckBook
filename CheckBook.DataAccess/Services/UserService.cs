using System.Collections.Generic;
using System.Linq;
using CheckBook.DataAccess.Context;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Enums;
using CheckBook.DataAccess.Model;
using CheckBook.DataAccess.Security;

namespace CheckBook.DataAccess.Services
{
    public static class UserService
    {
        /// <summary>
        /// Gets existing user by mail
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Existing user</returns>
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

        /// <summary>
        /// Creates new user if not exist
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Create result... Either success or duplicate user</returns>
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

        /// <summary>
        /// Updates existing user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="managerUpdate">If true allows more option to update from user param</param>
        public static void UpdateUser(UserData user, bool managerUpdate)
        {
            using (var db = new AppContext())
            {
                var userEntity = db.Users.First(x => x.Id == user.Id);
                userEntity.FirstName = user.FirstName;
                userEntity.LastName = user.LastName;
                if (managerUpdate)
                {
                    userEntity.Email = user.Email;
                    userEntity.UserRole = user.UserRole;
                }

                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    var passwordData = PasswordHelper.CreateHash(user.Password);
                    userEntity.PasswordSalt = passwordData.PasswordSalt;
                    userEntity.PasswordHash = passwordData.PasswordHash;
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Gets basic user info of all users
        /// </summary>
        /// <returns>List of basic user info of all users</returns>
        public static List<UserInfoData> GetUserInfoes()
        {
            using (var db = new AppContext())
            {
                return db.Users.Select(ToUserInfoData).ToList();
            }
        }

        /// <summary>
        /// Gets basic user info
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Basic user info</returns>
        public static UserInfoData GetUserInfo(int id)
        {
            using (var db = new AppContext())
            {
                return ToUserInfoData(db.Users.First(x => x.Id == id));
            }
        }

        /// <summary>
        /// Converts User entity into UserData
        /// </summary>
        /// <param name="user">User for conversion</param>
        /// <returns>Converted user</returns>
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

        /// <summary>
        /// Converts User entity into UserInfoData
        /// </summary>
        /// <param name="user">User for conversion</param>
        /// <returns>Converted user</returns>
        public static UserInfoData ToUserInfoData(User user)
        {
            return new UserInfoData()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserRole = user.UserRole
            };
        }
    }
}
