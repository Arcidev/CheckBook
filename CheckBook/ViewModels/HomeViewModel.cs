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
        public GridViewDataSet<PaymentData> PaymentHistory { get; set; }

        public HomeViewModel()
        {
            PaymentHistory = new GridViewDataSet<PaymentData>() { PageSize = 20 };
        }

        public override Task PreRender()
        {
            PaymentService.LoadPaymentHistory(GetUserId(), PaymentHistory);

            return base.PreRender();
        }
    }
}
