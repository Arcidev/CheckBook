
namespace DataAccess.Data
{
    public class UserData
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PasswordSalt { get; set; }

        public string PasswordHash { get; set; }

        public string Password { get; set; }

        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        public bool HasValidData
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                    string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Email))
                    return false;

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
