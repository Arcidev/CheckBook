
namespace DataAccess.Data
{
    public class PaymentGroupData
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string PayerName { get; set; }

        public decimal Value { get; set; }

        public string ValueText { get { return string.Format("{0} CZK", Value); } }
    }
}
