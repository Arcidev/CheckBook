using DataAccess.Enums;

namespace DataAccess.Data
{
    public class UserInfoData
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public UserRoles UserRole { get; set; }

        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        public UserInfoData()
        {
            UserRole = UserRoles.User;
        }
    }
}
