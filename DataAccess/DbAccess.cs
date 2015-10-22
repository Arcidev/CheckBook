using DataAccess.Context;
using DataAccess.Data;
using DataAccess.Enums;
using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DataAccess
{
    public static class DbAccess
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

                db.Users.Add(new User()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = user.Password
                });

                db.SaveChanges();
                return CreateUserResult.Success;
            }
        }

        public static string CreateHash(string value)
        {
            
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static List<GroupData> GetGroups()
        {
            using (var db = new AppContext())
            {
                return db.Groups.OrderBy(x => x.Name).Select(ToGroupData).ToList();
            }
        }

        public static GroupData CreateGroup(string name, List<int> userIds)
        {
            if (name == null)
                name = "";

            using (var db = new AppContext())
            {
                var group = db.Groups.Add(new Group() { Name = name });
                db.SaveChanges(); // generates id

                foreach (var userId in userIds.Distinct())
                    db.UsersGroups.Add(new UserGroups() { UserId = userId, GroupId = group.Id });

                db.SaveChanges();
                return ToGroupData(group);
            }
        }

        public static List<UserData> GetUsers()
        {
            using (var db = new AppContext())
            {
                return db.Users.Select(ToUserData).ToList();
            }
        }

        public static List<int> GetGroupUserIds(int groupId)
        {
            using (var db = new AppContext())
            {
                return db.UsersGroups.Where(x => x.GroupId == groupId).Select(x => x.UserId).ToList();
            }
        }

        public static void RemoveUserFromGroup(int userId, int groupId)
        {
            using (var db = new AppContext())
            {
                var userGroup = db.UsersGroups.FirstOrDefault(x => x.GroupId == groupId);
                if (userGroup == null)
                    return;

                db.UsersGroups.Remove(userGroup);
                db.SaveChanges();
            }
        }

        public static GroupData UpdateGroup(int groupId, string name, List<int> userIds)
        {
            if (name == null)
                name = "";

            using (var db = new AppContext())
            {
                var group = db.Groups.FirstOrDefault(x => x.Id == groupId);
                if (group == null)
                    return null;

                group.Name = name;
                db.UsersGroups.RemoveRange(db.UsersGroups.Where(x => x.GroupId == groupId));
                foreach (var userId in userIds.Distinct())
                    db.UsersGroups.Add(new UserGroups() { UserId = userId, GroupId = group.Id });

                db.SaveChanges();
                return ToGroupData(group);
            }
        }

        public static void CreatePayment(int payerId, List<int> debtorIds, decimal value)
        {
            using (var db = new AppContext())
            {
                var duplicateDebtors = db.Payments.Where(x => x.UserId == payerId && debtorIds.Contains(x.DebtorId)).ToList();
                foreach(var duplicateDebtor in duplicateDebtors)
                    duplicateDebtor.Value += value;

                var duplicateIds = duplicateDebtors.Select(x => x.DebtorId);
                foreach (var debtorId in debtorIds.Where(x => !duplicateIds.Contains(x)))
                {
                    db.Payments.Add(new Payment()
                    {
                        UserId = payerId,
                        DebtorId = debtorId,
                        Value = value
                    });
                }

                db.SaveChanges();
            }
        }

        public static List<PaymentData> GetDebtors(int payerId)
        {
            using (var db = new AppContext())
            {
                return db.Payments.Where(x => x.UserId == payerId).Select(ToPaymentData).ToList();
            }
        }

        public static List<PaymentData> GetPayers(int debtorId)
        {
            using (var db = new AppContext())
            {
                return db.Payments.Where(x => x.DebtorId == debtorId).Select(ToPaymentData).ToList();
            }
        }

        public static void PayDebt(int payerId, int debtorId, decimal value)
        {
            using (var db = new AppContext())
            {
                var payment = db.Payments.FirstOrDefault(x => x.DebtorId == debtorId && payerId == x.UserId);
                if (payment == null)
                    return;

                if (payment.Value <= value)
                    db.Payments.Remove(payment);
                else
                    payment.Value -= value;

                db.SaveChanges();
            }
        }

        private static GroupData ToGroupData(Group group)
        {
            return new GroupData() { Id = group.Id, Name = group.Name };
        }

        private static UserData ToUserData(User user)
        {
            return new UserData()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = user.Password
            };
        }

        private static PaymentData ToPaymentData(Payment payment)
        {
            return new PaymentData() { DebtorId = payment.DebtorId, UserId = payment.UserId, Value = payment.Value };
        }
    }
}
