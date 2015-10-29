using DotVVM.Framework.Runtime.Filters;
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
        public GridViewDataSet<PaymentHistoryData> PaymentHistory { get; set; }

        public HomeViewModel()
        {
            PaymentHistory = new GridViewDataSet<PaymentHistoryData>() { PageSize = 20 };
        }

        public override Task PreRender()
        {
            var paymentHistory = PaymentService.GetPaymentHistory(GetUserId());
            PaymentHistory.Items = paymentHistory;
            PaymentHistory.PageIndex = 0;
            PaymentHistory.TotalItemsCount = paymentHistory.Count;

            return base.PreRender();
        }
    }
}
