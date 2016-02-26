using System;
using CheckBook.DataAccess.Enums;

namespace CheckBook.DataAccess.Data
{
    public class PaymentData
    {
        public int? Id { get; set; }

        public int UserId { get; set; }

        public string ImageUrl => "/identicon/user-" + UserId;

        public string Name { get; set; }

        public decimal? Amount { get; set; }
        
    }
}
