using DataAccess.Context;
using DataAccess.Data;
using DataAccess.Model;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Services
{
    public static class GroupService
    {
        public static List<GroupData> GetGroups()
        {
            using (var db = new AppContext())
            {
                return db.Groups.OrderBy(x => x.Name).Select(ToGroupData).ToList();
            }
        }

        public static GroupData CreateGroup(string name, List<int> userIds)
        {
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

        public static List<int> GetGroupUserIds(int groupId)
        {
            using (var db = new AppContext())
            {
                return db.UsersGroups.Where(x => x.GroupId == groupId).Select(x => x.UserId).ToList();
            }
        }

        public static List<UserPaymentData> GetGroupUsersForPayment(int groupId)
        {
            using (var db = new AppContext())
            {
                var groupUsers = db.UsersGroups.Where(x => x.GroupId == groupId).Select(x => x.User);
                return groupUsers.Select(ToUserPaymentData).ToList();
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

        public static GroupData ToGroupData(Group group)
        {
            return new GroupData() { Id = group.Id, Name = group.Name };
        }

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
