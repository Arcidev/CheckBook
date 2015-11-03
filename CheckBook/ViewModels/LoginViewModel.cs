using CheckBook.Helpers;
using DotVVM.Framework.ViewModel;

namespace CheckBook.ViewModels
{
    public class LoginViewModel : DotvvmViewModelBase
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "All fields must contain some value";
                return;
            }

            var identity = LoginHelper.GetClaimsIdentity(Email, Password);
            if (identity == null)
            {
                ErrorMessage = "Invalid Email or Password";
                return;
            }

            Context.OwinContext.Authentication.SignIn(identity);
            Context.Redirect("Home");
        }

        public void Register()
        {
            Context.Redirect("Register");
        }
    }
}
