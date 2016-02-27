using System;
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
        /// Gets the user with specified e-mail address.
        /// </summary>
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

        ///// <summary>
        ///// Creates new user if not exist
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns>Create result... Either success or duplicate user</returns>
        //public static CreateUserResult CreateUser(UserData user)
        //{
        //    using (var db = new AppContext())
        //    {
        //        var email = user.Email.Trim().ToLower();
        //        var dbUser = db.Users.FirstOrDefault(x => x.Email == email);
        //        if (dbUser != null)
        //            return CreateUserResult.UserAlreadyExists;

        //        var passwordData = PasswordHelper.CreateHash(user.Password);

        //        db.Users.Add(new User()
        //        {
        //            Email = email,
        //            FirstName = user.FirstName,
        //            LastName = user.LastName,
        //            PasswordSalt = passwordData.PasswordSalt,
        //            PasswordHash = passwordData.PasswordHash,
        //            UserRole = user.UserRole
        //        });

        //        db.SaveChanges();
        //        return CreateUserResult.Success;
        //    }
        //}

        /// <summary>
        /// Updates the user data.
        /// </summary>
        public static void UpdateUserProfile(UserInfoData user, int userId)
        {
            using (var db = new AppContext())
            {
                var entity = db.Users.Find(userId);

                // update first and last name
                entity.FirstName = user.FirstName;
                entity.LastName = user.LastName;
                entity.ImageUrl = user.ImageUrl;
                
                // update the password
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    var passwordData = PasswordHelper.CreateHash(user.Password);
                    entity.PasswordSalt = passwordData.PasswordSalt;
                    entity.PasswordHash = passwordData.PasswordHash;
                }

                // update the e-mail and check e-mail uniqueness
                if (db.Users.Any(u => u.Id != userId && u.Email == user.Email))
                {
                    throw new Exception($"The user with e-mail address '{user.Email}' already exists!");
                }
                entity.Email = user.Email;

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
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
