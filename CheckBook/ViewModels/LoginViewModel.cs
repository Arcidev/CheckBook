using CheckBook.Helpers;
using DotVVM.Framework.ViewModel;

namespace CheckBook.ViewModels
{
    public class LoginViewModel : DotvvmViewModelBase
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public void Login()
        {
            var identity = LoginHelper.GetClaimsIdentity(Email, Password);
            if (identity == null)
                return;

            Context.OwinContext.Authentication.SignIn(identity);
            Context.Redirect("Home");
        }

        public void Register()
        {
            Context.Redirect("Register");
        }
    }
}
