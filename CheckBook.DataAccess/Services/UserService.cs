﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CheckBook.DataAccess.Context;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Enums;
using CheckBook.DataAccess.Model;
using CheckBook.DataAccess.Security;
using DotVVM.Framework.Controls;

namespace CheckBook.DataAccess.Services
{
    public static class UserService
    {
        /// <summary>
        /// Gets the user with specified e-mail address.
        /// </summary>
        public static UserWithPasswordData GetUserWithPassword(string email)
        {
            using (var db = new AppContext())
            {
                email = email.Trim().ToLower();

                return db.Users
                    .Select(ToUserWithPasswordData)
                    .FirstOrDefault(x => x.Email == email);
            }
        }

        /// <summary>
        /// Gets the user profile.
        /// </summary>
        public static UserInfoData GetUserInfo(int id)
        {
            using (var db = new AppContext())
            {
                return db.Users
                    .Select(ToUserInfoData)
                    .First(x => x.Id == id);
            }
        }

        /// <summary>
        /// Gets the user profile.
        /// </summary>
        public static void LoadUserInfos(GridViewDataSet<UserInfoData> dataSet)
        {
            using (var db = new AppContext())
            {
                var users = db.Users
                    .Select(ToUserInfoData);

                dataSet.LoadFromQueryable(users);
            }
        }

        /// <summary>
        /// Updates the user data (from the Settings page).
        /// </summary>
        public static void UpdateUserInfo(UserInfoData user, int userId)
        {
            using (var db = new AppContext())
            {
                var entity = db.Users.Find(userId);

                UpdateUserInfoCore(user, entity, db);

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Creates the or update user information (from the Manager page).
        /// </summary>
        public static void CreateOrUpdateUserInfo(UserInfoData user)
        {
            using (var db = new AppContext())
            {
                var entity = db.Users.Find(user.Id);
                if (entity == null)
                {
                    if (string.IsNullOrWhiteSpace(user.Password))
                    {
                        throw new Exception("The Password is required!");
                    }

                    entity = new User();
                    db.Users.Add(entity);
                }

                UpdateUserInfoCore(user, entity, db);
                entity.UserRole = user.UserRole;

                db.SaveChanges();
            }
        }

        private static void UpdateUserInfoCore(UserInfoData user, User entity, AppContext db)
        {
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
            if (db.Users.Any(u => u.Id != user.Id && u.Email == user.Email))
            {
                throw new Exception($"The user with e-mail address '{user.Email}' already exists!");
            }
            entity.Email = user.Email;
        }


        /// <summary>
        /// Converts the User entity into the UserWithPasswordData object
        /// </summary>
        public static Expression<Func<User, UserWithPasswordData>> ToUserWithPasswordData
        {
            get
            {
                return u => new UserWithPasswordData()
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PasswordHash = u.PasswordHash,
                    PasswordSalt = u.PasswordSalt,
                    UserRole = u.UserRole
                };
            }
        }

        /// <summary>
        /// Converts User entity into UserInfoData
        /// </summary>
        public static Expression<Func<User, UserInfoData>> ToUserInfoData
        {
            get
            {
                return u => new UserInfoData()
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ImageUrl = u.ImageUrl,
                    UserRole = u.UserRole
                };
            }
        }

    }
}
