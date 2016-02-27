
namespace CheckBook.DataAccess.Data
{
    public class UserData : UserInfoData
    {
        public string PasswordSalt { get; set; }

        public string PasswordHash { get; set; }

        public string Password { get; set; }

        public bool HasValidData
        {
            get
            {
                //if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                //    string.IsNullOrWhiteSpace(Email) || (string.IsNullOrWhiteSpace(Password) && Id == 0))
                //    return false;

                try
                {
                    new System.Net.Mail.MailAddress(Email);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

    }
}
