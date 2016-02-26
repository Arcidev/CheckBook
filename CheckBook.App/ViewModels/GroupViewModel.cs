using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Runtime.Filters;

namespace CheckBook.App.ViewModels
{
    [Authorize]
    public class GroupViewModel : AppViewModelBase
	{
	    public override string ActivePage => "home";

        public int GroupId => Convert.ToInt32(Context.Parameters["Id"]);

        public string GroupName { get; set; }

        public GridViewDataSet<PaymentGroupData> PaymentGroups { get; set; } = new GridViewDataSet<PaymentGroupData>()
        {
            PageSize = 40,
            SortDescending = true,
            SortExpression = nameof(PaymentGroupData.CreatedDate)
        };


        public override Task PreRender()
	    {
            // load group name
	        var userId = GetUserId();
	        GroupName = GroupService.GetGroup(GroupId, userId).Name;

            // load payment groups
            PaymentService.LoadPaymentGroups(GroupId, PaymentGroups);

	        return base.PreRender();
	    }
	}
}

