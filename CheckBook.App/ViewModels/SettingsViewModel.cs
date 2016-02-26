using DotVVM.Framework.Runtime.Filters;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;

namespace CheckBook.App.ViewModels
{
    [Authorize]
	public class SettingsViewModel : HeaderViewModel
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")]
        public string PasswordAgain { get; set; }

        public string Email { get; set; }

        public SettingsViewModel() : base("Settings") { }

        public override Task PreRender()
        {
            var user = UserService.GetUserInfo(GetUserId());
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;

            return base.PreRender();
        }

        public void UpdateSettings()
        {
            UserData user = new UserData()
            {
                Id = GetUserId(),
                FirstName = FirstName,
                LastName = LastName,
                Password = Password
            };

            UserService.UpdateUser(user, false);
        }
    }
}

