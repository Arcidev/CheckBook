using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Data
{
    public class PaymentGroupData
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public IEnumerable<PaymentData> Payments { get; set; }

        public decimal Value { get { return Payments.Sum(x => x.Value); } }
    }
}
