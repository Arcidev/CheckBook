using DataAccess.Context;
using DataAccess.Data;
using DataAccess.Model;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Services
{
    public static class GroupService
    {
        /// <summary>
        /// Gets all groups
        /// </summary>
        /// <returns>List of all groups</returns>
        public static List<GroupData> GetGroups()
        {
            using (var db = new AppContext())
            {
                return db.Groups.OrderBy(x => x.Name).Select(ToGroupData).ToList();
            }
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
                    db.UsersGroups.Add(new UserGroups() { UserId = userId, Group = group });

                db.SaveChanges();
                return ToGroupData(group);
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
                return db.UsersGroups.Where(x => x.GroupId == groupId).Select(x => x.UserId).ToList();
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
                var groupUsers = db.UsersGroups.Where(x => x.GroupId == groupId).Select(x => x.User);
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
                var userGroup = db.UsersGroups.FirstOrDefault(x => x.GroupId == groupId);
                if (userGroup == null)
                    return;

                db.UsersGroups.Remove(userGroup);
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
                db.UsersGroups.RemoveRange(db.UsersGroups.Where(x => x.GroupId == groupId));
                foreach (var userId in userIds.Distinct())
                    db.UsersGroups.Add(new UserGroups() { UserId = userId, GroupId = group.Id });

                db.SaveChanges();
                return ToGroupData(group);
            }
        }

        /// <summary>
        /// Converts Group entity into GroupData
        /// </summary>
        /// <param name="group">Group for conversion</param>
        /// <returns>Converted group</returns>
        public static GroupData ToGroupData(Group group)
        {
            return new GroupData() { Id = group.Id, Name = group.Name };
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
                Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                Value = 0
            };
        }
    }
}
