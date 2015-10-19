using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Runtime.Filters;
using DataAccess.Data;
using System.Linq;
using System.Collections.Generic;
using DotVVM.Framework.Controls;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using CheckBook.Models;

namespace CheckBook.ViewModels
{
    [Authorize]
	public class HomeViewModel : DotvvmViewModelBase
	{
        public GridViewDataSet<UserData> GroupUsers { get; private set; }

        public GridViewDataSet<UserPayment> Debtors { get; private set; }

        public GridViewDataSet<UserPayment> Payers { get; private set; }

        public List<GroupData> Groups { get; set; }

        public List<UserData> Users { get; private set; }

        public List<UserData> PaymentGroupUsers { get; private set; }

        public List<int> SelectedUsers { get; set; }

        public List<int> ExcludeUsers { get; set; }

        public int SelectedGroupId { get; set; }

        public int PaymentGroupId { get; set; }

        public int UserToPayId { get; set; }

        public int Payment { get; set; }

        public int DebtorId { get; private set; }

        public int DebtValue { get; set; }

        public bool GroupPopupVisible { get; private set; }

        public bool PaymentPopupVisible { get; private set; }

        public bool IsExistingGroup { get; private set; }

        public bool PayDebtVisible { get; private set; }

        public string GroupName { get; set; }

        public HomeViewModel()
        {
            GroupUsers = new GridViewDataSet<UserData>() { PageSize = 10 };
            Debtors = new GridViewDataSet<UserPayment>() { PageSize = 10 };
            Payers = new GridViewDataSet<UserPayment>() { PageSize = 10 };
            PaymentGroupUsers = new List<UserData>();
            ExcludeUsers = new List<int>();
            SelectedUsers = new List<int>();
        }

        public override Task PreRender()
        {
            Users = DataAccess.DbAccess.GetUsers();
            Groups = DataAccess.DbAccess.GetGroups();
            if (Groups.Any())
            {
                SelectedGroupId = Groups.First().Id;
                OnGroupChanged();
            }

            UpdateDebtors();
            UpdatePayers();

            return base.PreRender();
        }

        public void SignOut()
        {
            Context.OwinContext.Authentication.SignOut();
            Context.Redirect("Login", null);
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

        public void ShowPaymentPopup()
        {
            if (!PaymentPopupVisible && !Groups.Any())
                return;

            PaymentPopupVisible = !PaymentPopupVisible;
            if (PaymentPopupVisible)
            {
                if (ExcludeUsers.Any())
                    ExcludeUsers.Clear();

                PaymentGroupId = Groups.First().Id;
                OnPaymentGroupChanged(SelectedGroupId);

                UserToPayId = GetUserId();
            }
        }

        public void ManageGroup()
        {
            GroupData group = null;
            if (IsExistingGroup)
            {
                group = DataAccess.DbAccess.UpdateGroup(SelectedGroupId, GroupName, SelectedUsers);
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
                group = DataAccess.DbAccess.CreateGroup(GroupName, SelectedUsers);
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

        public void OnPaymentGroupChanged(int? groupId = null)
        {
            PaymentGroupUsers = GetGroupUsers(groupId ?? PaymentGroupId);
        }

        public void RemoveUser(int userId)
        {
            DataAccess.DbAccess.RemoveUserFromGroup(userId, SelectedGroupId);
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

        public void CreatePayment()
        {
            DataAccess.DbAccess.CreatePayment(UserToPayId, PaymentGroupUsers.Select(x => x.Id).Where(x => !ExcludeUsers.Contains(x)).ToList(), Payment);
            PaymentPopupVisible = false;
            UpdateDebtors();
        }

        public void HidePayDebtPopup()
        {
            PayDebtVisible = false;
            DebtorId = -1;
        }

        public void ShowPayDebtPopup(int userId)
        {
            var debtor = Debtors.Items.FirstOrDefault(u => u.UserId == userId);
            if (debtor == null)
                return;

            DebtorId = userId;
            PayDebtVisible = true;
            DebtValue = debtor.Value;
        }

        public void RemovePayment()
        {
            DataAccess.DbAccess.PayDebt(GetUserId(), DebtorId, DebtValue);
            var debtor = Debtors.Items.FirstOrDefault(u => u.UserId == DebtorId);
            if (debtor != null)
            {
                Debtors.Items.Remove(debtor);
                Debtors.TotalItemsCount--;
            }

            HidePayDebtPopup();
        }

        private List<UserData> GetGroupUsers(int groupId)
        {
            var userIds = DataAccess.DbAccess.GetGroupUserIds(groupId);
            return Users.Where(x => userIds.Contains(x.Id)).ToList();
        }

        private void UpdateDebtors()
        {
            var payments = DataAccess.DbAccess.GetDebtors(GetUserId());
            var users = Users.Join(payments, x => x.Id, y => y.DebtorId, (x, y) => new UserPayment(x, y)).ToList();
            Debtors.Items = users;
            Debtors.PageIndex = 0;
            Debtors.TotalItemsCount = users.Count;
        }

        private void UpdatePayers()
        {
            var payments = DataAccess.DbAccess.GetPayers(GetUserId());
            var users = Users.Join(payments, x => x.Id, y => y.UserId, (x, y) => new UserPayment(x, y)).ToList();
            Payers.Items = users;
            Payers.PageIndex = 0;
            Payers.TotalItemsCount = users.Count;
        }

        private int GetUserId()
        {
            return int.Parse(Context.OwinContext.Authentication.User.Identity.GetUserId());
        }
    }
}

