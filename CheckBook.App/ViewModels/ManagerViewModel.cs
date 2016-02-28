using System;
using System.Collections.Generic;
using System.Linq;
using DotVVM.Framework.Controls;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Enums;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.Runtime.Filters;

namespace CheckBook.App.ViewModels
{
    [Authorize(nameof(UserRole.Admin))]
	public class ManagerViewModel : AppViewModelBase
	{
        public GridViewDataSet<UserInfoData> Users { get; set; } = new GridViewDataSet<UserInfoData>()
        {
            SortExpression = nameof(UserInfoData.LastName),
            SortDescending = false,
            PageSize = 20
        };

        public UserInfoData EditedUser { get; set; } = new UserInfoData();

        public string UserAlertText { get; set; }


        public GridViewDataSet<GroupData> Groups { get; set; } = new GridViewDataSet<GroupData>()
        {
            SortExpression = nameof(GroupData.Name),
            SortDescending = false,
            PageSize = 20
        };

        public GroupData EditedGroup { get; set; } = new GroupData();

        public string GroupAlertText { get; private set; }

        public string GroupSearchText { get; private set; }

        public List<UserInfoData> GroupSearchResults { get; private set; }

        public List<UserInfoData> GroupUsers { get; set; }



        public override Task PreRender()
        {
            UserService.LoadUserInfos(Users);
            GroupService.LoadGroups(Groups);

            return base.PreRender();
        }

        public void ShowUserPopup(int? userId)
        {
            if (userId == null)
            {
                EditedUser = new UserInfoData() { UserRole = UserRole.User };
            }
            else
            {
                EditedUser = UserService.GetUserInfo(userId.Value);
            }

            Context.ResourceManager.AddStartupScript("$('div[data-id=user-detail]').modal('show');");
        }

        public void SaveUser()
        {
            try
            {
                UserService.CreateOrUpdateUserInfo(EditedUser);
                Context.ResourceManager.AddStartupScript("$('div[data-id=user-detail]').modal('hide');");
            }
            catch (Exception ex)
            {
                UserAlertText = ex.Message;
            }
        }

        public void DeleteUser()
        {
            try
            {
                UserService.DeleteUser(EditedUser.Id);
                Context.ResourceManager.AddStartupScript("$('div[data-id=user-detail]').modal('hide');");
            }
            catch (Exception ex)
            {
                UserAlertText = ex.Message;
            }
        }

        public void ShowGroupPopup(int? groupId)
        {
            if (groupId == null)
            {
                EditedGroup = new GroupData();
                GroupUsers = new List<UserInfoData>();
            }
            else
            {
                EditedGroup = GroupService.GetGroup(groupId.Value);
                GroupUsers = UserService.GetGroupUsers(groupId.Value);
            }

            // load users
            GroupSearchText = "";
            GroupSearch();

            Context.ResourceManager.AddStartupScript("$('div[data-id=group-detail]').modal('show');");
        }

        public void GroupSearch()
        {
            var currentGroupUsers = new HashSet<int>(GroupUsers.Select(u => u.UserId));

            GroupSearchResults = UserService.SearchUsers(GroupSearchText)
                .Where(u => !currentGroupUsers.Contains(u.UserId))
                .ToList();
        }

        public void GroupAddUser(UserInfoData user)
        {
            if (!GroupUsers.Any(u => u.UserId == user.Id))
            {
                GroupUsers.Add(user);
            }
            GroupSearch();
        }

        public void GroupRemoveUser(UserInfoData user)
        {
            GroupUsers.Remove(user);
            GroupSearch();
        }

        public void SaveGroup()
        {
            try
            {
                GroupService.CreateOrUpdateGroup(EditedGroup, GroupUsers);
                Context.ResourceManager.AddStartupScript("$('div[data-id=group-detail]').modal('hide');");
            }
            catch (Exception ex)
            {
                GroupAlertText = ex.Message;
            }
        }

        public void DeleteGroup()
        {
            try
            {
                GroupService.DeleteGroup(EditedGroup.Id);
                Context.ResourceManager.AddStartupScript("$('div[data-id=group-detail]').modal('hide');");
            }
            catch (Exception ex)
            {
                GroupAlertText = ex.Message;
            }
        }
    }
}
