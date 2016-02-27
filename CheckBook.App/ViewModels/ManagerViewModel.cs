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

        public GridViewDataSet<GroupData> Groups { get; set; } = new GridViewDataSet<GroupData>()
        {
            SortExpression = nameof(GroupData.Name),
            SortDescending = false,
            PageSize = 20
        };
        
        public UserInfoData EditedUser { get; set; }

        public string UserAlertText { get; set; }


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

        public void ShowGroupPopup(int? groupId)
        {
            
        }

    }
}
