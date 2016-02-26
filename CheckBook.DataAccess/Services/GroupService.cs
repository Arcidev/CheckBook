using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CheckBook.DataAccess.Context;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Model;

namespace CheckBook.DataAccess.Services
{
    public static class GroupService
    {
        /// <summary>
        /// Gets all groups.
        /// </summary>
        public static List<GroupData> GetGroups()
        {
            using (var db = new AppContext())
            {
                return db.Groups
                    .OrderBy(x => x.Name)
                    .Select(ToGroupData).ToList();
            }
        }

        /// <summary>
        /// Gets all groups for the specified user.
        /// </summary>
        public static List<GroupData> GetGroupsByUser(int userId)
        {
            using (var db = new AppContext())
            {
                return db.Groups
                    .Where(g => g.UserGroups.Any(ug => ug.UserId == userId))
                    .OrderBy(x => x.Name)
                    .Select(ToGroupData).ToList();
            }
        }

        /// <summary>
        /// Gets the group by ID.
        /// </summary>
        public static GroupData GetGroup(int groupId, int userId)
        {
            return GetGroupsByUser(userId).Single(g => g.Id == groupId);
        }


        /// <summary>
        /// Creates new group
        /// </summary>
        /// <param name="name">Group name</param>
        /// <param name="userIds">Users</param>
        /// <returns>Newly created group</returns>
        public static GroupData CreateGroup(string name, List<int> userIds)
        {
            using (var db = new AppContext())
            {
                var group = db.Groups.Add(new Group() { Name = name });

                foreach (var userId in userIds.Distinct())
                    db.UserGroups.Add(new UserGroup() { UserId = userId, Group = group });

                db.SaveChanges();
                return ToGroupData.Compile()(group);
            }
        }

        /// <summary>
        /// Gets group users id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>List of ids of group users</returns>
        public static List<int> GetGroupUserIds(int groupId)
        {
            using (var db = new AppContext())
            {
                return db.UserGroups.Where(x => x.GroupId == groupId).Select(x => x.UserId).ToList();
            }
        }

        /// <summary>
        /// Gets group users packed in UserPaymentData class
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>List of group users</returns>
        public static List<UserPaymentData> GetGroupUsersForPayment(int groupId)
        {
            using (var db = new AppContext())
            {
                var groupUsers = db.UserGroups.Where(x => x.GroupId == groupId).Select(x => x.User);
                return groupUsers.Select(ToUserPaymentData).ToList();
            }
        }

        /// <summary>
        /// Removes user from group
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        public static void RemoveUserFromGroup(int userId, int groupId)
        {
            using (var db = new AppContext())
            {
                var userGroup = db.UserGroups.FirstOrDefault(x => x.GroupId == groupId);
                if (userGroup == null)
                    return;

                db.UserGroups.Remove(userGroup);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Updates existing group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="name">New group name</param>
        /// <param name="userIds">New users for group</param>
        /// <returns></returns>
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
                db.UserGroups.RemoveRange(db.UserGroups.Where(x => x.GroupId == groupId));
                foreach (var userId in userIds.Distinct())
                    db.UserGroups.Add(new UserGroup() { UserId = userId, GroupId = group.Id });

                db.SaveChanges();
                return ToGroupData.Compile()(group);
            }
        }

        /// <summary>
        /// Converts Group entity into GroupData
        /// </summary>
        public static Expression<Func<Group, GroupData>> ToGroupData
        {
            get
            {
                return g => new GroupData()
                {
                    Id = g.Id,
                    Name = g.Name,
                    Currency = g.Currency,
                    TotalTransactions = g.PaymentGroups.Count(),
                    TotalSpending = g.PaymentGroups.Sum(pg => pg.Payments.Sum(p => (decimal?)p.Amount)) ?? 0

                    // We need to cast to (decimal?) because the result of the expression is NULL when there are no groups
                    // and null is not assignable in the property of decimal
                };
            }   
        } 

        /// <summary>
        /// Converts User entity into UserPaymentData
        /// </summary>
        /// <param name="user">User for conversion</param>
        /// <returns>Converted user</returns>
        public static UserPaymentData ToUserPaymentData(User user)
        {
            return new UserPaymentData()
            {
                UserId = user.Id,
                Name = $"{user.FirstName} {user.LastName}",
                Value = 0
            };
        }
    }
}
