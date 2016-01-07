using CheckBook.Helpers;
using DotVVM.Framework.ViewModel;

namespace CheckBook.ViewModels
{
    public class LoginViewModel : DotvvmViewModelBase
    {
        public string Email { get; set; }

        public string Password { get; set; }

        [Bind(Direction.ServerToClient)]
        public bool IsEmailValid { get; set; } = true;

        [Bind(Direction.ServerToClient)]
        public bool IsPasswordValid { get; set; } = true;

        public string ErrorMessage { get; set; }

        public void Login()
        {
            if (!Validate())
            {
                return;
            }

            var identity = LoginHelper.GetClaimsIdentity(Email, Password);
            if (identity == null)
            {
                ErrorMessage = "The credentials are incorrect.";
                return;
            }

            Context.OwinContext.Authentication.SignIn(identity);
            Context.Redirect("Home");
        }

        public bool Validate()
        {
            var valid = true;
            if (string.IsNullOrWhiteSpace(Email))
            {
                IsEmailValid = valid = false;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                IsPasswordValid = valid = false;
            }
            return valid;
        }
    }
}