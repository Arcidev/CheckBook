using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.Controls;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;

namespace CheckBook.App.ViewModels
{
    [Authorize]
	public class HomeViewModel : HeaderViewModel
    {
        public GridViewDataSet<PaymentData> Payments { get; set; }

        public GridViewDataSet<PaymentGroupData> PaymentGroups { get; set; }

        public HomeViewModel() : base("Home")
        {
            Payments = new GridViewDataSet<PaymentData>() { PageSize = 20 };
            PaymentGroups = new GridViewDataSet<PaymentGroupData>() { PageSize = 20 };
        }

        public override Task PreRender()
        {
            PaymentService.LoadPaymentHistory(GetUserId(), Payments);
            PaymentService.LoadPaymentGroups(GetUserId(), PaymentGroups);

            return base.PreRender();
        }
    }
}
