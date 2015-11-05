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

        public ManagerViewModel()
        {
            SelectedGroupId = -1;
            GroupUsers = new GridViewDataSet<UserInfoData>() { PageSize = 20 };
            SelectedUsers = new List<int>();
            User = new UserData();
        }

        public override Task PreRender()
        {
            Users = UserService.GetUsersInfo();
            Groups = GroupService.GetGroups();
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
            var groupUsers = GetGroupUsers(SelectedGroupId);
            GroupUsers.Items = groupUsers;
            GroupUsers.PageIndex = 0;
            GroupUsers.TotalItemsCount = groupUsers.Count;
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

        public void ShowUserPopup()
        {
            UserPopupVisible = !UserPopupVisible;
            if (UserPopupVisible)
            {
                ErrorMessage = null;
                User.FirstName = null;
                User.LastName = null;
                User.Email = null;
                Password = null;
                PasswordAgain = null;
                HasAdminRole = false;
            }
        }

        public void CreateUser()
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

            if (UserService.CreateUser(User) != CreateUserResult.Success)
            {
                ErrorMessage = "User with this email adress already exists";
                return;
            }
            
            ShowUserPopup();
        }

        private List<UserInfoData> GetGroupUsers(int groupId)
        {
            var userIds = GroupService.GetGroupUserIds(groupId);
            return Users.Where(x => userIds.Contains(x.Id)).ToList();
        }
    }
}
