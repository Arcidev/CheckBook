using CheckBook.Helpers;
using DataAccess.Data;
using DataAccess.Enums;
using DotVVM.Framework.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CheckBook.ViewModels
{
	public class RegisterViewModel : DotvvmViewModelBase
	{
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")]
        public string PasswordAgain { get; set; }

        public void Register()
        {
            var user = new UserData()
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = DataAccess.DbAccess.CreateHash(Password)
            };

            var r = DataAccess.DbAccess.CreateUser(user);
            if (r != CreateUserResult.Success)
                return;

            var identity = LoginHelper.GetClaimsIdentity(Email, DataAccess.DbAccess.CreateHash(Password));
            if (identity == null)
                return;

            Context.OwinContext.Authentication.SignIn(identity);
            Context.Redirect("Home");
        }
    }
}

