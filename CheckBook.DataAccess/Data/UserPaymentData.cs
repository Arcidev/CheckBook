
namespace CheckBook.DataAccess.Data
{
    public class UserPaymentData
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string ValueText { get { return string.Format("{0} CZK", Value); } }

        public decimal Value { get; set; }
    }
}
