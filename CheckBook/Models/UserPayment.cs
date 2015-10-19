
using DataAccess.Data;

namespace CheckBook.Models
{
    public class UserPayment
    {
        public int UserId { get; private set; }

        public string Name { get; private set; }

        public string ValueText { get { return string.Format("{0} CZK", Value); } }

        public int Value { get; set; }

        public UserPayment(UserData user, PaymentData payment)
        {
            Name = user.FullName;
            UserId = user.Id;
            Value = payment.Value;
        }

        public UserPayment() { }
    }
}