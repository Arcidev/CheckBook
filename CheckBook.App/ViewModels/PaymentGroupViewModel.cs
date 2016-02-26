using System;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Runtime.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.ViewModel;

namespace CheckBook.App.ViewModels
{
    [Authorize]
    public class PaymentGroupViewModel : AppViewModelBase
    {
        public override string ActivePage => "home";

        public PaymentGroupData Data { get; set; }

        public List<PaymentData> Payers { get; set; }

        public List<PaymentData> Debtors { get; set; }

        public decimal AmountDifference { get; set; }

        public string ErrorMessage { get; set; }
        
        public bool IsEditable { get; set; }

        public bool IsDeletable { get; set; }


        public override Task Load()
        {
            if (!Context.IsPostBack)
            {
                CreateOrLoadData();
            }
            return base.Load();
        }

        private void CreateOrLoadData()
        {
            // get group
            var userId = GetUserId();
            var groupId = Convert.ToInt32(Context.Parameters["GroupId"]);
            var group = GroupService.GetGroup(groupId, userId);

            // get or create payment group
            var paymentGroupId = Context.Parameters["Id"];
            if (paymentGroupId != null)
            {
                // load
                Data = PaymentService.GetPaymentGroup(Convert.ToInt32(paymentGroupId));
                IsEditable = IsDeletable = PaymentService.IsPaymentGroupEditable(userId, Convert.ToInt32(paymentGroupId));
            }
            else
            {
                // create new
                Data = new PaymentGroupData()
                {
                    GroupId = groupId,
                    CreatedDate = DateTime.Today,
                    Currency = group.Currency
                };
                IsEditable = true;
                IsDeletable = false;
            }

            // load payers and debtors
            Payers = PaymentService.GetPayers(groupId, Convert.ToInt32(paymentGroupId));
            Debtors = PaymentService.GetDebtors(groupId, Convert.ToInt32(paymentGroupId));
            Recalculate();
        }

        public void Save()
        {
            try
            {
                PaymentService.SavePaymentGroup(Data, Payers, Debtors);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            Context.Redirect("group", new { Id = Data.GroupId });
        }

        public void Delete()
        {
            try
            {
                PaymentService.DeletePaymentGroup(Data, Payers, Debtors);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            Context.Redirect("group", new { Id = Data.GroupId });
        }

        public void Recalculate()
        {
            AmountDifference = (Payers.Where(p => p.Amount != null).Sum(p => p.Amount) ?? 0) - (Debtors.Where(p => p.Amount != null).Sum(p => p.Amount) ?? 0);
        }

        public void GoBack()
        {
            Context.Redirect("group", new { Id = Data.GroupId });
        }
    }
}

