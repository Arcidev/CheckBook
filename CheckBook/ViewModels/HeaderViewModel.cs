using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace CheckBook.ViewModels
{
    [Authorize]
	public class HeaderViewModel : DotvvmViewModelBase
	{
        public bool IsAdmin { get; set; }

        public override Task PreRender()
        {
            IsAdmin = Context.OwinContext.Authentication.User.IsInRole("Admin");
            return base.PreRender();
        }

        public void SignOut()
        {
            Context.OwinContext.Authentication.SignOut();
            Context.Redirect("Login", null);
        }

        public int GetUserId()
        {
            return int.Parse(Context.OwinContext.Authentication.User.Identity.GetUserId());
        }

        public void RedirectHome()
        {
            Context.Redirect("Home");
        }

        public void RedirectPayment()
        {
            Context.Redirect("Payment");
        }

        public void RedirectManager()
        {
            Context.Redirect("Manager");
        }
    }
}

