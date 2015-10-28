﻿using DotVVM.Framework.Runtime.Filters;
using DataAccess.Data;
using System.Linq;
using System.Collections.Generic;
using DotVVM.Framework.Controls;
using System.Threading.Tasks;
using DataAccess.Services;

namespace CheckBook.ViewModels
{
    [Authorize]
	public class HomeViewModel : HeaderViewModel
    {
        public GridViewDataSet<UserInfoData> GroupUsers { get; private set; }

        public List<GroupData> Groups { get; set; }

        public List<UserInfoData> Users { get; private set; }

        public List<int> SelectedUsers { get; set; }

        public int SelectedGroupId { get; set; }

        public bool GroupPopupVisible { get; private set; }

        public bool IsExistingGroup { get; private set; }

        public string GroupName { get; set; }

        public HomeViewModel()
        {
            SelectedGroupId = -1;
            GroupUsers = new GridViewDataSet<UserInfoData>() { PageSize = 10 };
            SelectedUsers = new List<int>();
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
            IsExistingGroup = false;
            if (GroupName == null || GroupName.Any())
                GroupName = "";
            if (SelectedUsers.Any())
                SelectedUsers.Clear();
            GroupPopupVisible = !GroupPopupVisible;
        }

        public void ManageGroup()
        {
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
            var group = Groups.FirstOrDefault(x => x.Id == SelectedGroupId);
            if (group == null)
                return;

            GroupName = group.Name;
            SelectedUsers = GroupUsers.Items.Select(x => x.Id).ToList();
            IsExistingGroup = true;
            GroupPopupVisible = true;
        }

        private List<UserInfoData> GetGroupUsers(int groupId)
        {
            var userIds = GroupService.GetGroupUserIds(groupId);
            return Users.Where(x => userIds.Contains(x.Id)).ToList();
        }
    }
}

