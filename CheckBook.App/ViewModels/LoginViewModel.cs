using System.ComponentModel.DataAnnotations;
using CheckBook.App.Helpers;
using CheckBook.App.Helpers;
using DotVVM.Framework.ViewModel;

namespace CheckBook.App.ViewModels
{
    public class LoginViewModel : DotvvmViewModelBase
    {
        [Required(ErrorMessage = "The e-maill is required!")]
        [EmailAddress(ErrorMessage = "The e-mail is not valid!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password is required!")]
        public string Password { get; set; }
        
        public string ErrorMessage { get; set; }

        public void Login()
        {
            Context.FailOnInvalidModelState();

            var identity = LoginHelper.GetClaimsIdentity(Email, Password);
            if (identity == null)
            {
                ErrorMessage = "The credentials are incorrect.";
                return;
            }

            Context.OwinContext.Authentication.SignIn(identity);
            Context.Redirect("Home");
        }
        
    }
}