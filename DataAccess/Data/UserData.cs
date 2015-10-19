
namespace DataAccess.Data
{
    public class UserData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string FullName { get { return string.Format("{0} {1}", Name, Surname); } }

        public bool HasValidData
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Surname) ||
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
