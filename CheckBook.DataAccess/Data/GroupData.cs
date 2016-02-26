
using System.Net;

namespace CheckBook.DataAccess.Data
{
    public class GroupData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Currency { get; set; }

        public decimal TotalSpending { get; set; }

        public int TotalTransactions { get; set; }

        public string ImageUrl => "/identicon/group-" + WebUtility.UrlEncode(Id.ToString());
    }
}
