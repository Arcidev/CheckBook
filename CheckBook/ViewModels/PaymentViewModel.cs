using DataAccess.Data;
using DataAccess.Services;
using DotVVM.Framework.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckBook.ViewModels
{
	public class PaymentViewModel : HeaderViewModel
	{
        public GridViewDataSet<UserPaymentData> Debtors { get; private set; }

        public GridViewDataSet<UserPaymentData> Payers { get; private set; }

        public List<UserPaymentData> PaymentGroupUsers { get; private set; }

        public List<GroupData> Groups { get; set; }

        public List<UserInfoData> Users { get; private set; }

        public int UserToPayId { get; set; }

        public int DebtorId { get; private set; }

        public int PaymentGroupId { get; set; }

        public decimal SharedPayment { get; set; }

        public decimal DebtValue { get; set; }

        public bool PaymentPopupVisible { get; private set; }

        public bool PayDebtVisible { get; private set; }

        public PaymentViewModel()
        {
            Debtors = new GridViewDataSet<UserPaymentData>() { PageSize = 10 };
            Payers = new GridViewDataSet<UserPaymentData>() { PageSize = 10 };
            Users = new List<UserInfoData>();
            Groups = new List<GroupData>();
            PaymentGroupUsers = new List<UserPaymentData>();
        }

        public override Task PreRender()
        {
            Users = UserService.GetUsersInfo();
            Groups = GroupService.GetGroups();

            UpdateDebtors();
            UpdatePayers();

            return base.PreRender();
        }

        public void ShowPaymentPopup()
        {
            if (!PaymentPopupVisible && !Groups.Any())
                return;

            PaymentPopupVisible = !PaymentPopupVisible;
            if (PaymentPopupVisible)
            {
                PaymentGroupId = Groups.First().Id;
                OnPaymentGroupChanged();

                UserToPayId = GetUserId();
            }
        }

        public void CreatePayment()
        {
            PaymentService.CreatePayment(UserToPayId, PaymentGroupUsers, SharedPayment);
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
            PaymentService.PayDebt(GetUserId(), DebtorId, DebtValue);
            var debtor = Debtors.Items.FirstOrDefault(u => u.UserId == DebtorId);
            if (debtor != null)
            {
                Debtors.Items.Remove(debtor);
                Debtors.TotalItemsCount--;
            }

            HidePayDebtPopup();
        }

        public void OnPaymentGroupChanged()
        {
            PaymentGroupUsers = GroupService.GetGroupUsersForPayment(PaymentGroupId);
        }

        private void UpdateDebtors()
        {
            var payments = PaymentService.GetDebtors(GetUserId());
            Debtors.Items = payments;
            Debtors.PageIndex = 0;
            Debtors.TotalItemsCount = payments.Count;
        }

        private void UpdatePayers()
        {
            var payments = PaymentService.GetPayers(GetUserId());
            Payers.Items = payments;
            Payers.PageIndex = 0;
            Payers.TotalItemsCount = payments.Count;
        }
    }
}

