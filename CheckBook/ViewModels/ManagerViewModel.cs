using System.Collections.Generic;
using System.Linq;
using DotVVM.Framework.Controls;
using DataAccess.Data;
using System.Threading.Tasks;
using DataAccess.Services;
using DataAccess.Enums;
using DotVVM.Framework.Runtime.Filters;

namespace CheckBook.ViewModels
{
    [Authorize("Admin")]
	public class ManagerViewModel : HeaderViewModel
	{
        public GridViewDataSet<UserInfoData> GroupUsers { get; private set; }

        public GridViewDataSet<UserInfoData> UserInfoes { get; private set; }

        public List<GroupData> Groups { get; set; }

        public List<UserInfoData> Users { get; private set; }

        public List<int> SelectedUsers { get; set; }

        public UserData User { get; set; }

        public string Password { get; set; }

        public string PasswordAgain { get; set; }

        public string GroupName { get; set; }

        public string ErrorMessage { get; set; }

        public int SelectedGroupId { get; set; }

        public bool GroupPopupVisible { get; private set; }

        public bool UserPopupVisible { get; private set; }

        public bool IsExistingGroup { get; private set; }

        public bool HasAdminRole { get; set; }

        public ManagerViewModel() : base("Manager")
        {
            SelectedGroupId = -1;
            GroupUsers = new GridViewDataSet<UserInfoData>() { PageSize = 20 };
            UserInfoes = new GridViewDataSet<UserInfoData>() { PageSize = 20 };
            SelectedUsers = new List<int>();
            User = new UserData();
        }

        public override Task PreRender()
        {
            Users = UserService.GetUserInfoes();
            Groups = GroupService.GetGroups();
            UserInfoes.LoadFromQueryable(Users.AsQueryable());

            if (Groups.Any())
            {
                if (SelectedGroupId == -1)
                    SelectedGroupId = Groups.First().Id;
                OnGroupChanged();
            }

            return base.PreRender();
        }

        public void ShowGroupPopup()
        {
            ErrorMessage = "";
            IsExistingGroup = false;
            if (GroupName != null)
                GroupName = null;
            if (SelectedUsers.Any())
                SelectedUsers.Clear();
            GroupPopupVisible = !GroupPopupVisible;
        }

        public void ManageGroup()
        {
            if (string.IsNullOrWhiteSpace(GroupName))
            {
                ErrorMessage = "Group name must contain some value";
                return;
            }

            GroupData group = null;
            if (IsExistingGroup)
            {
                group = GroupService.UpdateGroup(SelectedGroupId, GroupName, SelectedUsers);
                var groupInGrid = Groups.FirstOrDefault(x => x.Id == SelectedGroupId);
                if (group == null || groupInGrid == null)
                {
                    GroupPopupVisible = false;
                    return;
                }

                Groups.Remove(groupInGrid);
                OnGroupChanged();
            }
            else
            {
                group = GroupService.CreateGroup(GroupName, SelectedUsers);
                if (!Groups.Any())
                {
                    SelectedGroupId = group.Id;
                    OnGroupChanged();
                }
            }

            Groups.Add(group);
            Groups.OrderBy(x => x.Name);
            GroupPopupVisible = false;
        }

        public void OnGroupChanged()
        {
            GroupUsers.LoadFromQueryable(GetGroupUsers(SelectedGroupId));
        }

        public void RemoveUser(int userId)
        {
            GroupService.RemoveUserFromGroup(userId, SelectedGroupId);
            var user = GroupUsers.Items.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                GroupUsers.Items.Remove(user);
                GroupUsers.TotalItemsCount--;
            }
        }

        public void ManageGroupPopup()
        {
            ErrorMessage = "";
            GroupName = Groups.First(x => x.Id == SelectedGroupId).Name;
            SelectedUsers = GroupUsers.Items.Select(x => x.Id).ToList();
            IsExistingGroup = true;
            GroupPopupVisible = true;
        }

        public void ShowUserPopup(int? userId = null)
        {
            UserPopupVisible = !UserPopupVisible;
            if (UserPopupVisible)
            {
                UserInfoData userInfo = userId != null ? Users.First(x => x.Id == userId) : null;
                ErrorMessage = null;
                User.Id = userInfo != null ? userInfo.Id : 0;
                User.FirstName = userInfo != null ? userInfo.FirstName : null;
                User.LastName = userInfo != null ? userInfo.LastName : null;
                User.Email = userInfo != null ? userInfo.Email : null;
                Password = null;
                PasswordAgain = null;
                HasAdminRole = userInfo != null ? userInfo.UserRole == UserRoles.Admin : false;
            }
        }

        public void ManageUser()
        {
            if (Password != PasswordAgain)
            {
                ErrorMessage = "Password and Password Again must be the same and must contain some value";
                return;
            }

            User.Password = Password;
            User.UserRole = HasAdminRole ? UserRoles.Admin : UserRoles.User;
            if (!User.HasValidData)
            {
                ErrorMessage = "All fields must contain some value";
                return;
            }

            if (User.Id != 0)
                UserService.UpdateUser(User);
            else if (UserService.CreateUser(User) != CreateUserResult.Success)
            {
                ErrorMessage = "User with this email adress already exists";
                return;
            }
            
            ShowUserPopup();
        }

        private IQueryable<UserInfoData> GetGroupUsers(int groupId)
        {
            var userIds = GroupService.GetGroupUserIds(groupId);
            return Users.Where(x => userIds.Contains(x.Id)).AsQueryable();
        }
    }
}
