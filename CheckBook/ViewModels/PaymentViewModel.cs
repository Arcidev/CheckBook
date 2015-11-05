using DataAccess.Data;
using DataAccess.Services;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Runtime.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckBook.ViewModels
{
    [Authorize]
    public class PaymentViewModel : HeaderViewModel
	{
        public GridViewDataSet<UserPaymentData> Debtors { get; private set; }

        public GridViewDataSet<UserPaymentData> Payers { get; private set; }

        public List<UserPaymentData> PaymentGroupUsers { get; private set; }

        public List<GroupData> Groups { get; set; }

        public List<UserInfoData> Users { get; private set; }

        public string ErrorMessage { get; set; }

        public string PaymentDescription { get; set; }

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

            PaymentService.LoadDebtorsAndPayers(GetUserId(), Payers, Debtors);

            return base.PreRender();
        }

        public void ShowPaymentPopup()
        {
            if (!PaymentPopupVisible && !Groups.Any())
            {
                ErrorMessage = "No group found in the system";
                return;
            }

            ErrorMessage = null;
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
            if (string.IsNullOrWhiteSpace(PaymentDescription))
            {
                ErrorMessage = "You have to provide a description";
                return;
            }

            PaymentService.CreatePayment(UserToPayId, PaymentDescription, PaymentGroupUsers, SharedPayment);
            PaymentPopupVisible = false;
            PaymentService.LoadDebtorsAndPayers(GetUserId(), Payers, Debtors);
        }

        public void HidePayDebtPopup()
        {
            PayDebtVisible = false;
            DebtorId = -1;
        }

        public void ShowPayDebtPopup(int userId)
        {
            DebtorId = userId;
            PayDebtVisible = true;
            DebtValue = Debtors.Items.First(u => u.UserId == userId).Value;
        }

        public void RemovePayment()
        {
            PaymentService.PayDebt(GetUserId(), DebtorId, DebtValue);
            PaymentService.LoadDebtorsAndPayers(GetUserId(), Payers, Debtors);

            HidePayDebtPopup();
        }

        public void OnPaymentGroupChanged()
        {
            PaymentGroupUsers = GroupService.GetGroupUsersForPayment(PaymentGroupId);
        }
    }
}

